## UK Driver Licence OCR Bot

Sample Bot that can detect a UK Driver License and display details from it.  This sample bot uses https://docs.microsoft.com/en-us/azure/cognitive-services/computer-vision/

## Use Visual Studio 

### Build and debug
1. Get a valid Computer Vision Subscription Key: https://docs.microsoft.com/en-us/azure/cognitive-services/Computer-vision/vision-api-how-to-topics/howtosubscribe
2. Add the key to the web.config
3. Add the Computer Vision OCR API endpoint to the web.config (based on the region above eg: https://westeurope.api.cognitive.microsoft.com/vision/v1.0/ocr)
4. download and run [botframework-emulator](https://emulator.botframework.com/)
5. connect the emulator to http://localhost:3987

### Test

Upload an image attachment of a license or post a URL to a valid image.  You can also try this with other images that contain text eg. number plates.


