# RentMyCPU

An app made to compute tasks from other nodes in the network using your CPU and you earn money for that.

# Project architecture

`RentMyCPU.WebView`: The client app.  
`RentMyCPU.Shared`: shared resources used by other projects.  
`RentMyCPU.Web`: the web app that manages all of the nodes.  
`RentMyCPU.WebView.RuntimeComponent`: a component that is used inside app webview to handle IAP and notifications

# How it works

This app works with webassembly files. These files are executed in a sandboxed environment (because uwp apps are sandboxed by default).
You have the ability to execute a webassembly file on many nodes.
To differentiate the node on which the webassembly file is executed, a numeric parameter is passed.
