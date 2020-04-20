
## Objective
* To Provide an API to retreive/store/delete messages for UX Team
* To use the API of persistence team




## PseudoCode


* To retrive message from database
```csharp
public void RetrieveMessage(fromTime,tillTime){

  

  //call the API of IPersist


}
```

* To delete Message from database
```csharp
public void DeleteMessage(ip,fromTime,tillTime){

  

  //call the API of IPersist


}
```

* To store message to database
```csharp
public void StoreMessage(message,fromIp,toIp){

  //get timestamp, date and add these to message 

  //call API of IPersist with new data


}
```

## Explanation
* All these methods are member functions of class Messager
* Each of these functions will be called by UX which in turn will call API of persistence with some change in arguments
