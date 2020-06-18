var jsBridge = {
    execute: function(methodName, parameter) {
        methodName = Pointer_stringify(methodName)
        parameter = Pointer_stringify(parameter)

        var jsonObj = {}
        jsonObj.methodName = methodName
        jsonObj.parameter = parameter

        var argsmentString = JSON.stringify(jsonObj)
        var event = new CustomEvent('unityMessage', { detail: argsmentString })
        window.dispatchEvent(event)
    }
}

mergeInto(LibraryManager.library, jsBridge);