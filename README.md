# RentMyCPU

An app made to compute task from other node in the network with your CPU and earn money from that.

# Project architecture

`RentMyCPU.WebView`: The client app.  
`RentMyCPU.Shared`: shared resources used by other projects.  
`RentMyCPU.Web`: the web app that manage all the nodes.  
`RentMyCPU.WebView.RuntimeComponent`: a component that is used inside app webview to handle IAP and notifications

# How it's work

This app work with webassembly files. Theses files are executed in a sandboxed environment (because uwp apps are sandboxed by default).
You have the ability to execute a webassembly file on many nodes.
To differentiate the node on which the webassembly file is executed, a numeric parameter is passed.
