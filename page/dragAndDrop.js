document.addEventListener('DOMContentLoaded', function () {
    var dropArea = document.getElementById('screen')

    function confirmVrmFile(files) {
        let length = files.length, file

        for (let i = 0; i < length; i++) {
            file = files[i]

            let isVrmFile = file.name.endsWith('.vrm')

            if (isVrmFile) {
                let reader = new FileReader()
                reader.onload = function () {
                    let source = this.result
                    let bytes = new Uint8Array(source)
                    let len = source.byteLength
                    let byteString = ""
                    for (var i = 0; i < len; i++) {
                        byteString += String.fromCharCode(bytes[i])
                    }
                    var base64String = window.btoa(byteString)
                    sendUnity(base64String)
                };
                reader.readAsArrayBuffer(file);
                break
            }
        }
    }

    function sendUnity(base64String) {
        let splitLength = 1000
        let len = parseInt(base64String.length / splitLength)
        unityInstance.SendMessage("VrmProvider", "ByteLength", len + 1)
        for (let i = 0; i < len; i++) {
            let next = base64String.substr(i * splitLength, splitLength)
            unityInstance.SendMessage("VrmProvider", "AddRange", next)
        }
        let last = base64String.substr(
            splitLength * len,
            base64String.length % splitLength
        )
        unityInstance.SendMessage("VrmProvider", "AddRange", last)
    }

    dropArea.addEventListener('dragover', function (ev) {
        ev.preventDefault()
        ev.dataTransfer.dropEffect = 'copy'
        dropArea.classList.add('dragover')
    })

    dropArea.addEventListener('dragleave', function () {
        dropArea.classList.remove('dragover')
    })

    dropArea.addEventListener('drop', function (ev) {
        ev.preventDefault()
        dropArea.classList.remove('dragover')
        confirmVrmFile(ev.dataTransfer.files)
    })
})