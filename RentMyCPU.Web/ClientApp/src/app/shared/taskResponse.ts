import { TaskContext } from "./taskContext";

export class TaskResponse {
    TaskId: string;
    Elapsed: number;
    Parameter: number;
    Result: number;
    Context: TaskContext;
}