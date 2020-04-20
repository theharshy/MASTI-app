# Objective


To Start the process to get the screen from the client, sending and retreiving the image from persistance module and Testing after Integration stage.

## GetSharedScreen
When the server (professor) asks/initates the student to share the screen UI calls the GetSharedScreen function is called.

### Dependencies:
- Inter team:
    - UI team.
- Intra team:
    - Signalling class.
    - ReceiveData class.

### Class
```csharp
public void ImageProcessingServer
```
### Method
```csharp
public void GetSharedScreen(Ipaddress ipaddressClient)
{
    //call the signalling fns
    IServerSignalling.SignalClient(ipaddressClient, start);
    //initiating the thread to receive data
    System.Threading.Thread myThread = System.Threading.Thread(new System.Threading.ThreadStart(ReceiveScreen()));
    myThread.Start();
}
```
##### Explaination:
- Whenever server asks the client to start sharing the screen GetSharedScreen(ipaddresClient) is called by UI team.
- This will initialize the thread for receiving image from Client.
- It also send the signalls to Signalling Class Informing about the start sharing fro the corresponding client.


## Store and Retreive Image from Persistance
It was discussed in the class that only messages will be stored and not the images but since it is mentioned in the Project Architecture I am giving brief implementation of storing and retreiving images. 

### Dependencies
- Inter team:
    - UI team
    - Persistance team
    - Schema team

### Class
```csharp
public void ImageProcessingServer
```

### Method
```csharp
public void ImageSaving(Id id,bitmap image)
{
    //call the schema team to encode the image to string
    String imageString = ISchema.Encode({id : image});
    //call the persistance class to store the image
    IStorage.store(imageString, time);
}

public bitmap RetreiveImage(Id id)
{
    //retreive the image from presistance team
    imageDictionary = IStorage.retreive(id, startTime, endTime);
    //decode the image using schema team 
    decodedImage = ISchema.Decode( imageDictionary[1], true);
    return decodeImage[1];
}
```
##### Explaination
- When the server clicks the save button of the image, the image and id was received from the UI team and image is encoded to string with the help of schema team and stored using API of persistance team.
- When the server asks to retreive the image, UI team sends the id and RetreiveImage gets the image from persistance team in string format and then send that string to schema team to decode it to bitmap image and send that image to UI to publish.


## Testing : Integration
The main reasion for testing the code after integration is check whether the passage of data between is as expected.
- Check what happens if client is not sending the images
- Check ordering of images sent to server.
