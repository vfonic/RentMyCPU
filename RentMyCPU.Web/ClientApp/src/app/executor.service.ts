declare function postMessage(message: any): void;
declare var WebAssembly: any;

import { Injectable } from "@angular/core";
import { ReceivedTask } from "./shared/receivedTask";
import { TaskResponse } from "./shared/taskResponse"; 
import { WebworkerService } from "./webworker.service";
import { BehaviorSubject } from "rxjs";


@Injectable()
export class ExecutorService {
    private currentTaskExecuted: Promise<string> = null; 
    public currentTaskExecutedId: string = null;
    
    private isExecutingTaskSource = new BehaviorSubject<boolean>(false);
    public  isExecutingTask = this. isExecutingTaskSource.asObservable();

    constructor(private webworkerService: WebworkerService) {

    }

    private base64ToArrayBuffer(base64) {
        var binary_string = window.atob(base64);
        var len = binary_string.length;
        var bytes = new Uint8Array(len);
        for (var i = 0; i < len; i++) {
            bytes[i] = binary_string.charCodeAt(i);
        }
        return bytes.buffer;
    }

    isValid(b64: string): Promise<boolean> {
        return new Promise((resolve) => {
            var buffer = this.base64ToArrayBuffer(b64);
            (window as any).WebAssembly.instantiate(buffer, {
                env: { writechar: function (c) { } }
            }).then(() => {
                resolve(true);
            }).catch(() => {
                resolve(false);
            })
        })
    }

    execute(task: ReceivedTask): Promise<TaskResponse> {
        return new Promise((resolve, reject) => {
            if(this.currentTaskExecuted != null){
                reject("Cannot run 2 tasks in parallel");
            }
            var buffer = this.base64ToArrayBuffer(task.WasmB64);
            var func = (input) => {
                var context = { WritedChars: [] };
                WebAssembly.instantiate(input.buffer, {
                    env: {
                        writechar: function (c) { 
                            context.WritedChars.push(c);
                        }
                    }
                }).then(results => {
                    var start = new Date().getTime();
                    var result = results.instance.exports.run(input.task.Parameter);
                    var end = new Date().getTime();
                    var response = {
                        Result: result,
                        Elapsed: end - start,
                        Parameter: input.task.Parameter,
                        TaskId: input.task.TaskId,
                        Context: context
                    };
                    postMessage(JSON.stringify(response));
                });
                return "";
            };
            var input = {
                buffer: buffer,
                task: task
            };
            var promise = this.webworkerService.run(func, input);
            this.currentTaskExecuted = promise;
            this.currentTaskExecutedId = task.TaskId;
            promise.then((res) => {
                this.currentTaskExecuted = null;
                this.currentTaskExecutedId = null;
                resolve(JSON.parse(res) as TaskResponse);
            }).catch((e) => {
                this.currentTaskExecuted = null;
                this.currentTaskExecutedId = null;
                reject(e.message);
            });
        })
    }

    stop(){
        if(this.currentTaskExecuted == null){
            return;
        }
        this.webworkerService.terminate(this.currentTaskExecuted);
        this.currentTaskExecuted = null; 
        this.currentTaskExecutedId = null;
        this.isExecutingTaskSource.next(false);
    }
}
