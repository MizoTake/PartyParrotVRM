
let model, ctx, videoWidth, videoHeight, video, canvas, scatterGLHasInitialized = false, scatterGL
let videoDevices = {}
let isChangingCamera = false
let unityCanvas = {}
let capturer = new CCapture( { format: 'gif', workersPath: 'js/' } )
let captureIndex = 0

const mobile = isMobile()
const renderPointcloud = mobile === false

function isMobile() {
  const isAndroid = /Android/i.test(navigator.userAgent)
  const isiOS = /iPhone|iPad|iPod/i.test(navigator.userAgent)
  return isAndroid || isiOS
}

function drawPath(ctx, points, closePath) {
  const region = new Path2D()
  region.moveTo(points[0][0], points[0][1])
  for (let i = 1; i < points.length; i++) {
    const point = points[i]
    region.lineTo(point[0], point[1])
  }

  if (closePath) {
    region.closePath()
  }
  ctx.stroke(region)
}

async function setupCamera(parameter) {
  video = document.querySelector('video')

  let videoParam = {
    width: mobile ? undefined : parameter.resolution[0],
    height: mobile ? undefined : parameter.resolution[1]
  }
  if (parameter.device === undefined) {
    
  } else {
    videoParam.deviceId = parameter.device.deviceId
  }

  let stream = {}

  try {
    stream = await navigator.mediaDevices.getUserMedia({
      'audio': false,
      'video': videoParam,
    }).then(
      stream => (video.srcObject = stream),
      err => console.log(err)
    )
  } catch (e) {
    
  }


  return new Promise((resolve) => {
    video.onloadedmetadata = () => {
      resolve(video)
    }
  })
}

async function renderPrediction(parameter) {
  const predictions = await model.estimateFaces(video)
  ctx.drawImage(video, 0, 0, videoWidth, videoHeight, 0, 0, canvas.width, canvas.height)

  if (predictions.length > 0) {

    unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName, JSON.stringify(predictions))

    predictions.forEach(prediction => {
      const keypoints = prediction.scaledMesh

      for (let i = 0; i < keypoints.length; i++) {
        const x = keypoints[i][0]
        const y = keypoints[i][1]

        ctx.beginPath()
        ctx.arc(x, y, 1 /* radius */, 0, 2 * Math.PI)
        ctx.fill()
      }
    })

    if (renderPointcloud && scatterGL != null) {
      const pointsData = predictions.map(prediction => {
        let scaledMesh = prediction.scaledMesh
        return scaledMesh.map(point => ([-point[0], -point[1], -point[2]]))
      })

      let flattenedPointsData = []
      for (let i = 0; i < pointsData.length; i++) {
        flattenedPointsData = flattenedPointsData.concat(pointsData[i])
      }
      const dataset = new ScatterGL.Dataset(flattenedPointsData)

      if (!scatterGLHasInitialized) {
        scatterGL.render(dataset)
      } else {
        scatterGL.updateDataset(dataset)
      }
      scatterGLHasInitialized = true
    }
  }

  requestAnimationFrame(function () {
    if (isChangingCamera) {
      return
    } else {
      renderPrediction(parameter)
    }
  })

}

async function setup(parameter) {
  await setupCamera(parameter)
  video.play()

  videoWidth = video.videoWidth
  videoHeight = video.videoHeight
  video.width = videoWidth
  video.height = videoHeight

  canvas = document.getElementById('output')
  canvas.width = videoWidth
  canvas.height = videoHeight
  const canvasContainer = document.querySelector('.canvas-wrapper')
  canvasContainer.style = `width: ${videoWidth}px height: ${videoHeight}px`
  canvasContainer.style.display == "block"

  ctx = canvas.getContext('2d')
  ctx.translate(canvas.width, 0)
  ctx.scale(-1, 1)
  ctx.fillStyle = '#32EEDB'
  ctx.strokeStyle = '#32EEDB'
  ctx.lineWidth = 0.5

  unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName)
}

async function trackingStart(parameter) {
  model = await facemesh.load()
  renderPrediction(parameter)
}

async function sendCameraList(parameter) {
  await navigator.mediaDevices.enumerateDevices().then(function (devices) {
    videoDevices = devices.filter(device => device.kind === 'videoinput')
    unityInstance.SendMessage(parameter.cameraSwitcherObject, parameter.firstCameraCallback, video.srcObject.id)
    unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName, JSON.stringify(videoDevices))
  })
}

async function selectedCamera(parameter) {
  video.pause()
  video.srcObject = null
  isChangingCamera = true

  parameter.device = {}
  parameter.device.deviceId = parameter.selectedDeviceId
  await setupCamera(parameter)
  video.play()

  isChangingCamera = false

  renderPrediction(parameter)
}

function cameraPreview(parameter) {
  const preview = document.getElementById('canvas-wrapper')

  if (preview.style.display == "block") {
    preview.style.display = "none"
  } else {
    preview.style.display = "block"
  }
}

function downloadStartGif(parameter) {
  unityCanvas = document.getElementById('#canvas')
  capturer = new CCapture( { format: 'gif', workersPath: 'js/' } )
  capturer.start()
  unityInstance.SendMessage(parameter.callbackGameObjectName, parameter.callbackFunctionName)
}

function captureFrame(parameter) {
  capturer.capture(unityCanvas)
  if (parameter.index === parameter.max) {
    capturer.stop()
    capturer.save()
  }
}

function recieveMessage(event) {
  var data = JSON.parse(event.detail)
  var methodName = data.methodName
  var parameter = data.parameter
  try {
    parameter = JSON.parse(parameter)
  } catch (e) {
    parameter = null
  }
  eval(`${methodName}(parameter)`)
}

window.addEventListener('unityMessage', recieveMessage, false)
