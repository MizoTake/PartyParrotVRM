var ftjsPlugin = {
    ExecuteJs: function(id, methodName, callbackGameObjectName) {
        id = Pointer_stringify(id);
        methodName = Pointer_stringify(methodName);
        callbackGameObjectName = Pointer_stringify(callbackGameObjectName);

        var jsonObj = {};
        jsonObj.Id = id;
        jsonObj.MethodName = methodName;
        jsonObj.CallbackGameObjectName = callbackGameObjectName;

        var messageString = JSON.stringify(jsonObj);
        window.postMessage(messageString, "*");
        console.log("messageString" + messageString)
    }
}

mergeInto(LibraryManager.library, ftjsPlugin);