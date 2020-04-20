
# Interaction with UI team
**Author**

**Sai Nishanth Vaka (111501038)**

## Introduction
* Design and implentation of necessary functions for UI to interact with messaging module .


## Objectives
* Design and Implement function ```SendMessage``` for the use of *UI to send message* .
* Design and Implement function ```SubscribeToDataReciever``` for the use of *UI to get messages* (on subsribing they can recieve messages).
* Design and Implement function ```SubscribeToStatusReceiver``` for the use of *UI to know the status of messages* (on subsribing they can kniw the status of message sucess or fail).


## Psuedocode


* For UI to send message

```csharp
void SendMessage(String message, String toIp,String toPort, String datetime,messageId);
{
	//add time stamp to message and using schemaObj encode the message
	//using ICommunicate send this encoded message to communication
}
```

* For UI need to Subscribe for recieving messages 

```csharp
public void SubscribeToDataReciever(DataRecieverHandler handler)
{
	//receiveHandler=new DataRecieverHandler(handler)
	//pass our callback functions using subscriber functions of communicator class
}
```

* For UI to status of the sent message

```csharp	
public void SubscribeToStatusReceiver(DataStatusHandler handler)
{
	//statusHandler=new DataStatusHandler(handler)
	//pass our callback functions using subscriber functions of communicator class	
}
```