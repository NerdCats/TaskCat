# TaskCat #

## Type Controllers.AssetProviderController

 Asset Providers are responsible for fetching eligible assets for a certain job 



---
#### Method Controllers.AssetProviderController.Search(System.Double,System.Double,System.String,System.Nullable{System.Double},System.Int32,TaskCat.Model.Asset.SearchStrategy)

 Endpoint to search for assets available 

|Name | Description |
|-----|------|
|lat: | latitude of designated location to find asset around |
|lon: | longitude of designated location to find asset around |
|address: | address of designated location to find asset around |
|radius: | radius around designated location to find asset from |
|limit: | limit the numbers of results, default is 10 |
|strategy: | search strategy to go for, default is QUICK |
**Returns**:  A List of available assets with their locations 



---
## Type Controllers.Auth.AccountController

 Account (User And Asset related Controller) 



---
#### Method Controllers.Auth.AccountController.#ctor(TaskCat.Lib.Auth.IAccountContext)

 Account Controller Constructor 

|Name | Description |
|-----|------|
|accountContext: | AuthRepository is an Authentication Repository Instance |


---
#### Method Controllers.Auth.AccountController.Register(TaskCat.Data.Model.Identity.Registration.RegistrationModelBase)

 Registers an User or Asset in the system 

|Name | Description |
|-----|------|
|userModel: | UserModel or AssetModel to register into system |
**Returns**: 



---
#### Method Controllers.Auth.AccountController.ConfirmEmail(System.String,System.String)

 Route to confirm email address after a registration 

|Name | Description |
|-----|------|
|userId: | userId created against the email |
|code: | email verficiation token |
**Returns**: 



---
#### Method Controllers.Auth.AccountController.ResendConfirmationEmail(System.String)

 Resend Confirmation Mails from the system if needed 

|Name | Description |
|-----|------|
|userId: | user that needs a confirmation mail |
**Returns**:  Sends back a response that states the status of the email request 



---
#### Method Controllers.Auth.AccountController.Profile(System.String)

 View Public Profile on any user or asset 

|Name | Description |
|-----|------|
|userId: | User Id for user or asset |
**Returns**:  Public profile for given user id, some properties are masked if you log in as anonymous as User or Asset 



---
#### Method Controllers.Auth.AccountController.GetAssignedJobs(System.String,System.Int32,System.Int32,System.String,TaskCat.Data.Model.JobState,MongoDB.Driver.SortDirection)

 Gets Assigned Jobs to an Asset sorted by CreatedTime 

|Name | Description |
|-----|------|
|userId: | userId for the Asset to find assigned jobs, would only work for Administrator and BackendAdministrator roles |
|pageSize: | Page size of the request, default is 10 |
|page: | Desired page number |
|fromDateTime: | Results should be fetched from this date, if sent null, all results are fetched back regardless of Creation Time of the Job |
|jobStateUpto: | Highest Job State to be fetched, default is IN_PROGRESS, that means by default ENQUEUED and IN_PROGRESS jobs would be fetched |
|sortDirection: | default sort by CreatedTime direction, usually set at descending |
**Returns**:  A paginated set of results for Assigned job against an user id 



---
#### Method Controllers.Auth.AccountController.GetAllPaged(System.Int32,System.Int32,System.Boolean)

 Get All Users 

|Name | Description |
|-----|------|
|pageSize: | Page size for the result, default is 10 |
|page: | Page number of the result |
|envelope: | enveope in paged result, default is false, when enveloped, usually provide page number ,total and next and previous page links |
**Returns**: 



---
#### Method Controllers.Auth.AccountController.GetAllOdata(System.Int32,System.Int32,System.Boolean)

 Odata powered query to get users 



> It would basically be a collection where all the odata queries are done with standard TaskCat Paging Supported Odata query are $count, $filter, $orderBy, $skip, $top 

|Name | Description |
|-----|------|
|pageSize: | pageSize for a single page |
|page: | page number to be fetched |
|envelope: | By default this is true, given false, the result comes as not paged |
**Returns**:  A list of Users And Assets that complies with the query 



---
#### Method Controllers.Auth.AccountController.Update(TaskCat.Data.Model.Identity.Profile.IdentityProfile)

 profile 

|Name | Description |
|-----|------|
|model: | Model |
**Returns**: 



---
#### Method Controllers.Auth.AccountController.Check

 Check availibility of a suggsted [[|F:TaskCat.Lib.Constants.CheckPropertyNames.EMAIL]] , [[|F:TaskCat.Lib.Constants.CheckPropertyNames.PHONENUMBER]] or an [[|F:TaskCat.Lib.Constants.CheckPropertyNames.EMAIL]]

**Returns**:  returns whether the suggested value is available 



---
#### Method Controllers.Auth.AccountController.UploadAvatar

 Upload an avatar/profile picture for an account 

**Returns**:  return a FileUploadModel 



---
## Type Controllers.CommentController

 Default controller to serve comments for any referenced entity 



---
#### Method Controllers.CommentController.#ctor(TaskCat.Lib.Comments.ICommentService)

 Initializes a default instance of CommentController 

|Name | Description |
|-----|------|
|service: | ICommentService to facilitate comment features |


---
#### Method Controllers.CommentController.Get(System.Int32,System.Int32,System.Boolean)

 Odata route to query comments 

|Name | Description |
|-----|------|
|pageSize: |Page size to return results in. |
|page: |Page number to return. |
|envelope: |Boolean trigger to envelope or package the data in or not. |
**Returns**: 



---
#### Method Controllers.CommentController.Get(System.String)

 Get a single comment by id. 

|Name | Description |
|-----|------|
|id: |Comment id to be fetched.|
**Returns**: Comment with specified id.



---
#### Method Controllers.CommentController.GetComments(System.String,System.String,System.Int32,System.Int32)

 Get comments by entity type and reference id which is ordered by create time. 

|Name | Description |
|-----|------|
|entityType: |Entity type the comment is associated with.|
|refId: |Reference Id for the comment.|
|pageSize: |Desired page size.|
|page: |Desired page number.|
**Returns**: 



---
#### Method Controllers.CommentController.Post(TaskCat.Data.Entity.Comment)

 Post request to create a comment. 

|Name | Description |
|-----|------|
|comment: |Comment to be created. |
**Returns**: 



---
#### Method Controllers.CommentController.Delete(System.String)

 Delete request to delete a comment. 

|Name | Description |
|-----|------|
|id: |Delete to be created.|
**Returns**: 



---
## Type Controllers.DropPointController

 Registers and manages drop points for a certain user 



---
#### Method Controllers.DropPointController.Get(System.String,System.String)

 Gets a Drop Point based on id and userId 

|Name | Description |
|-----|------|
|id: ||
|userId: | Optional param, needed if an Administrator is actually looking for someone else's DropPoint |
**Returns**:  A DropPoint matching the query 



---
#### Method Controllers.DropPointController.GetDropPointNameSuggestions

 Get a list of predefined name suggestions for the drop points 

**Returns**:  List of predefined name suggestions for the drop points 



---
#### Method Controllers.DropPointController.Search(System.String,System.String,System.Int32,System.Int32,System.Boolean)

 Gets a list of drop points based on a search on Address and Name 

|Name | Description |
|-----|------|
|query: | Text to search in the Address and Name in a Drop Point |
|userId: | Optional, this field is populated automatically from authentication header if not provided If an administrator wants to search in one specific user's droppoint list he would have to use this field |
|pageSize: | page size to return result in |
|page: | page number |
|envelope: | Defines whether the result should be sent back as a paged result or not |
**Returns**:  Returns either a [[|T:TaskCat.Model.Pagination.PageEnvelope`1]] or a [[|T:System.Collections.Generic.List`1]]



---
#### Method Controllers.DropPointController.GetOdata(System.Int32,System.Int32,System.Boolean)

 Odata endpoint for only Administrator and BackendOfficeAdmins to execute queries needed for components that needs a comprehensive list 

|Name | Description |
|-----|------|
|pageSize: | page size |
|page: | page number |
|envelope: | Defines whether the result should be sent back as a paged result or not |
**Returns**:  Returns either a [[|T:TaskCat.Model.Pagination.PageEnvelope`1]] or a [[|T:System.Collections.Generic.List`1]]



---
#### Method Controllers.DropPointController.Post(TaskCat.Data.Entity.DropPoint)

 Submit a new drop point 

|Name | Description |
|-----|------|
|value: | DropPoint Value to be submitted |
**Returns**:  The newly created drop point 



---
#### Method Controllers.DropPointController.Put(TaskCat.Data.Entity.DropPoint)

 Update a certain drop point 

|Name | Description |
|-----|------|
|value: | Drop point to be updated, only an administrator can save change any users drop point userId field to other userId field |
**Returns**:  Returns the modified drop point 



---
#### Method Controllers.DropPointController.Delete(System.String,System.String)

 Deletes a drop point based on id and optionally user id 

|Name | Description |
|-----|------|
|id: | Sepcific drop point id to be deleted |
|userId: | Optional, if an Administrator wants to delete a specific id of an user, he can use this field |
**Returns**:  Deleted drop point 



---
## Type Controllers.JobController

 Controller to Post Custom Jobs, List, Delete and Update Jobs 



---
#### Method Controllers.JobController.Get(System.String)

 Returns A specific job request based on id or human readable id This endpoint can be accessed being authorized or non-authorized If you are authorized as a Administrator or BackOfficeAdmin, You'd be able to search by id and hrid If you're accessing the non-authorized endpoint, you can only search by hrid 

|Name | Description |
|-----|------|
|id: | job id to get |
**Returns**: Job 



---
#### Method Controllers.JobController.List(System.String,System.Int32,System.Int32,System.Boolean)

 List Jobs mostly with just type filter 

|Name | Description |
|-----|------|
|type: | Job type to be filtered by |
|pageSize: | Page Size to return results |
|page: | page number to return results |
|envelope: | envelope the job results in pagination header |
**Returns**:  Return Jobs matching type filter 



---
#### Method Controllers.JobController.ListOdata(System.Int32,System.Int32,System.Boolean)

 Odata powered query to get jobs 



> It would basically be a collection where all the odata queries are done with standard TaskCat Paging Supported Odata query are $count, $filter, $orderBy, $skip, $top 

|Name | Description |
|-----|------|
|pageSize: | pageSize for a single page |
|page: | page number to be fetched |
|envelope: | By default this is true, given false, the result comes as not paged |
**Returns**:  A list of Jobs that complies with the query 



---
#### Method Controllers.JobController.GenerateInvoiceForAJob(System.String)

 Generate an invoice against the job hrid given in here 

|Name | Description |
|-----|------|
|jobhrid: | Human Readable ID for a job |
**Returns**:  A invoice for a job 



---
#### Method Controllers.JobController.Post(TaskCat.Data.Model.Api.JobModel)

 Post a generic job payload 

**Returns**: 



---
#### Method Controllers.JobController.Notify(System.String)



|Name | Description |
|-----|------|
|jobId: ||
**Returns**: 



---
#### Method Controllers.JobController.Claim(System.String)

 Claim a job as a server 



> This is used to claim a job 

|Name | Description |
|-----|------|
|jobId: | Id for a job |
**Returns**:  Returns a replace result that replaces the JobServedBy field 



---
#### Method Controllers.JobController.Update(System.String,System.String,Marvin.JsonPatch.JsonPatchDocument{TaskCat.Data.Model.JobTask})

 Partial update to a specific task under a job 



> Patch update to a specific task to set a partial update like changing assetRef or task state 

|Name | Description |
|-----|------|
|jobId: | Job Id the task is associated with |
|taskId: ||
|taskPatch: ||
**Returns**:  Returns a ReplaceOneResult based on the update 



---
#### Method Controllers.JobController.CancelJob(TaskCat.Model.Job.JobCancellationRequest)

 Cancel a job with specific job id 

**Returns**: 



---
#### Method Controllers.JobController.RestoreJob(System.String)

 Restores a freezed job with a specific job id 



> A job can freeze itslef if its deleted or cancelled All outstanding and future changes for a job would be rejected unless it is restored 

**Returns**: 



---
#### Method Controllers.JobController.UpdateOrder(System.String,TaskCat.Data.Model.OrderModel)

 Update a order inside a job 

|Name | Description |
|-----|------|
|jobId: | Job Id the task is associated with |
|orderModel: | OrderModel derivative to update |
**Returns**:  Returns a ReplaceOneResult based on the update 



---
#### Method Controllers.OrderController.PostOrder(TaskCat.Data.Model.OrderModel,TaskCat.Data.Model.Order.OrderCreationOptions)

 Default endpoint to create an Order 

|Name | Description |
|-----|------|
|model: | OrderModel to be submitted, this can be anything that is inherited from OrderModel class |
|opt: | Order create options, an admin can create an order and claim it himself too |
**Returns**: 



---
#### Method Controllers.OrderController.GetAllSupportedOrder

 Gets List of the all supported order types 

**Returns**:  List of supported orders 



---
#### Method Controllers.OrderController.PostSupportedOrder(TaskCat.Data.Entity.SupportedOrder)

 Add a supported order in the system 

|Name | Description |
|-----|------|
|supportedOrder: ||
**Returns**: 



---
#### Method Controllers.OrderController.GetSupportedOrder(System.String)

 Get a supported order by id 

|Name | Description |
|-----|------|
|id: | id of the supported order entity |
**Returns**:  Supported Order of that respective id 



---
#### Method Controllers.OrderController.UpdateSupportedOrder(TaskCat.Data.Entity.SupportedOrder)

 Update a supported order 

|Name | Description |
|-----|------|
|order: | SupportedOrder that needs to be updated |
**Returns**:  Updated Supported Order 



---
#### Method Controllers.OrderController.DeleteSupportedOrder(System.String)

 Delete a supported Order 

|Name | Description |
|-----|------|
|id: | id of supported order to delete |
**Returns**:  Deleted Supported Order 



---
## Type Controllers.PaymentController

 `PaymentController`  exposes all services provided by a  `IPaymentManager` 



---
#### Method Controllers.PaymentController.#ctor(TaskCat.Lib.Payment.IPaymentManager,TaskCat.Data.Lib.Payment.IPaymentService,TaskCat.Lib.Job.IJobRepository)

 `PaymentController`  constructor 

|Name | Description |
|-----|------|
|manager: | `IPaymentManager instance to be injected in the controller` |


---
#### Method Controllers.PaymentController.Get

 Get all the supported payment methods 

**Returns**:  Returns all the payment method added in  `PaymentManager` 



> Get all the supported payment methods 



---
#### Method Controllers.PaymentController.Process(System.String)

 Process Payment request of a job 

|Name | Description |
|-----|------|
|jobid: | this is the job id that should get its payment processed |
**Returns**: 



---
## Type Controllers.StorageController

 StorageController is the official controller to upload image and other files to TaskCat storage service for various purpose 



---
#### Method Controllers.StorageController.#ctor(TaskCat.Lib.Storage.IStorageRepository)

 StorageController Constructor 

|Name | Description |
|-----|------|
|repository: ||


---
#### Method Controllers.StorageController.Image

 Post Endpoint for uploading images Its a multi-part form data request. Post the image in path "image" 

**Returns**: 



---
#### Method Controllers.StorageController.DeleteImage(System.String)

 Deletes a file from Storage Service 

|Name | Description |
|-----|------|
|fileName: | desired file name to be deleted |
**Returns**:  File Deletion Status as a FileDeleteModel 



---
## Type Lib.AssetProvider.DefaultAssetProvider

 Default implementation of asset provider, essentially provides assets for a job 



---
## Type Lib.Asset.IAssetProvider

 INearestAssetProvider is the default interface for providing needed assets in time 



---
#### Method Lib.Asset.IAssetProvider.FindEligibleAssets(TaskCat.Model.Asset.AssetSearchRequest)

 Finds Eligible Assets based on an Asset Request 

|Name | Description |
|-----|------|
|assetRequest: ||
**Returns**: 



---
## Type Lib.Comments.CommentService

 Default implementation of ICommentService 



---
## Type Lib.Comments.ICommentService

 Default implementation for Comment repository 



---
#### Method Lib.Comments.ICommentService.GetByRefId(System.String,System.String,System.Int32,System.Int32)

 Get a comment feed based on a reference id and an entity type. 

|Name | Description |
|-----|------|
|refId: |Reference Id for the comment.|
|entityType: |Entity type for the comment reference.|
|page: |Page number to be fetched.|
|pageSize: |Page size to be used.|
**Returns**: 



---
#### Method Lib.Comments.ICommentService.IsValidEntityTypeForComment(System.String)

 Determines whether this entity type is valid for commenting 

|Name | Description |
|-----|------|
|entityType: |Entity type for the comment reference.|
**Returns**: 



---
## Type Lib.DataProtection.MachineKeyDataProtectionProvider

 Used to provide the data protection services that are derived from the MachineKey API. It is the best choice of data protection when you application is hosted by ASP.NET and all servers in the farm are running with the same Machine Key values. 



---
#### Method Lib.DataProtection.MachineKeyDataProtectionProvider.Create(System.String[])

 Returns a new instance of IDataProtection for the provider. 

|Name | Description |
|-----|------|
|purposes: |Additional entropy used to ensure protected data may only be unprotected for the correct purposes.|
**Returns**: An instance of a data protection service



---
#### Method Lib.Exceptions.EntityDeleteException.#ctor(System.String)

 Base constructor to create a EntityDeleteException with a message 

|Name | Description |
|-----|------|
|message: ||


---
#### Method Lib.Exceptions.EntityDeleteException.#ctor(System.String,System.String)

 Initiate a EntityDeleteException with a message generated from entityType 

|Name | Description |
|-----|------|
|entityType: ||
|identifier: ||


---
#### Method Lib.Exceptions.EntityDeleteException.#ctor(System.Type,System.String)

 Initiate a EntityDeleteException with a message generated from entityType 

|Name | Description |
|-----|------|
|entityType: ||
|identifier: ||


---
#### Method Lib.Exceptions.EntityDeleteException.#ctor(System.String,System.Exception)

 Initiate EntityDeleteException with an inner exception 

|Name | Description |
|-----|------|
|description: ||
|inner: ||


---
#### Method Lib.Exceptions.EntityNotFoundException.#ctor(System.String)

 Base constructor to create a EntityNotFoundException with a message 

|Name | Description |
|-----|------|
|message: |Exception message that describes what element is missing|


---
#### Method Lib.Exceptions.EntityNotFoundException.#ctor(System.String,System.String)

 Initiate a EntityNotFoundException with a message generated from stringified entityType 

|Name | Description |
|-----|------|
|entityType: ||
|identifier: ||


---
#### Method Lib.Exceptions.EntityNotFoundException.#ctor(System.Type,System.String)

 Initiate a EntityNotFoundException with a message generated from entityType 

|Name | Description |
|-----|------|
|entityType: ||
|identifier: ||


---
#### Method Lib.Exceptions.EntityNotFoundException.#ctor(System.String,System.Exception)

 Initiate EntityNotFoundException with an inner exception 

|Name | Description |
|-----|------|
|description: ||
|inner: ||


---
#### Method Lib.Exceptions.EntityNotFoundException.#ctor(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)

 Initiate EntityNotFoundException with serialization info and context 

|Name | Description |
|-----|------|
|info: ||
|context: ||


---
#### Method Lib.Exceptions.EntityUpdateException.#ctor(System.String)

 Base constructor to create a EntityUpdateException with a message 

|Name | Description |
|-----|------|
|message: ||


---
#### Method Lib.Exceptions.EntityUpdateException.#ctor(System.String,System.String)

 Initiate a EntityUpdateException with a message generated from entityType 

|Name | Description |
|-----|------|
|entityType: ||
|identifier: ||


---
#### Method Lib.Exceptions.EntityUpdateException.#ctor(System.Type,System.String)

 Initiate a EntityUpdateException with a message generated from entityType 

|Name | Description |
|-----|------|
|entityType: ||
|identifier: ||


---
#### Method Lib.Exceptions.EntityUpdateException.#ctor(System.String,System.Exception)

 Initiate EntityUpdateException with an inner exception 

|Name | Description |
|-----|------|
|description: ||
|inner: ||


---
## Type Lib.Exceptions.OrderCalculationException

 Generic Exception to define a calculation exception for IOrderCalculationService 



---
## Type Lib.Exceptions.ServerErrorException

 Represents an error with the details described as an ErrorDocument 



---
## Type Lib.Payment.Manual.CashOnDeliveryPaymentMethod

 Manual payment processor for cash when delivery processes 



---
#### Property Lib.Payment.Manual.CashOnDeliveryPaymentSettings.IsAdditionalFeeSetOnPercentage

 Gets or sets a value indicating whether to "additional fee" is specified as percentage. true - percentage, false - fixed value. 



---
#### Property Lib.Payment.Manual.CashOnDeliveryPaymentSettings.AdditionalFee

 Additional fee 



---
## Type Lib.Payment.Manual.TransactMode

 Represents manual payment processor transaction mode 



---
#### Field Lib.Payment.Manual.TransactMode.Pending

 Pending 



---
#### Field Lib.Payment.Manual.TransactMode.Authorize

 Authorize 



---
#### Field Lib.Payment.Manual.TransactMode.AuthorizeAndCapture

 Authorize and capture 



---
#### Property Lib.Payment.PaymentManager.AllowRePostingPayments

 Gets or sets a value indicating whether customers are allowed to repost (complete) payments for redirection payment methods 



---
#### Property Lib.Payment.PaymentManager.BypassPaymentMethodSelectionIfOnlyOne

 Gets or sets a value indicating whether we should bypass 'select payment method' page if we have only one payment method 



---
#### Property Lib.Payment.PaymentManager.AllPaymentMethods

 Return All Available Payment Methods 



---
#### Property Lib.Payment.PaymentManager.ActivePaymentMethodKeys

 Gets or sets a system names of active payment methods 



---
## Type Lib.Payment.PaymentService

 Default Payment Service for all installer services through a IPaymentManager 



---
## Type Lib.Storage.IBlobService

 Interface to define a Blob Storage Service 



---
#### Method Lib.Storage.IBlobService.UploadBlob(System.Net.Http.HttpContent,System.String,System.Collections.Generic.IEnumerable{System.String})

 Upload single blob data from HttpContent 

|Name | Description |
|-----|------|
|content: ||
|filterPropertyName: ||
|supportedFileTypes: ||
**Returns**: 



---
#### Method Lib.Storage.IBlobService.UploadBlobs(System.Net.Http.HttpContent)

 Upload Blobs data from HttpContent 

|Name | Description |
|-----|------|
|content: ||
**Returns**: 



---
#### Method Lib.Storage.IBlobService.DownloadBlob(System.String)

 Download a file from Blob with a blob name 

|Name | Description |
|-----|------|
|blobName: ||
**Returns**: 



---
#### Method Lib.Storage.IBlobService.TryDeleteBlob(System.String)

 Tries to delete a blob/file from storage 

|Name | Description |
|-----|------|
|fileName: ||
**Returns**: 



---
#### Method Lib.Storage.IBlobService.DeleteBlob(System.String)

 Deletes a blob/file from storage with proper status 

|Name | Description |
|-----|------|
|fileName: ||
**Returns**:  Returns a FileDeleteModel object with proper deletion status 



---
## Type Lib.Utility.IdentityExtensions

 Extension methods to help with Identity entities 



---
#### Method Lib.Utility.IdentityExtensions.IsAdmin(System.Security.Principal.IPrincipal)

 Returns whether an user is an admin backend office admin or not 

|Name | Description |
|-----|------|
|user: | Identity user that is to be checked |
**Returns**: 



---
## Type Model.Asset.AssetSearchRequest

 Standard Asset Search Request based on location for a IAssetProvider 



---
#### Property Model.Asset.AssetSearchRequest.Location

 Location class to define searches around a place 



---
#### Property Model.Asset.AssetSearchRequest.Radius

 Radius in meters to limit search area 



---
#### Property Model.Asset.AssetSearchRequest.Limit

 Limit results of search, default is 10 



---
#### Property Model.Asset.AssetSearchRequest.Strategy

 Search Strategy, based on search strategy, asset provider should find out assets for a particular job 



---
## Type Model.Asset.SearchStrategy

 Search Strategy to define asset search paradigm in a IAssetProvider 



---
#### Field Model.Asset.SearchStrategy.QUICK

 QUICK search paradigm is faster but less intrusive, Multiple trials and search zone changes are not supported 



---
#### Field Model.Asset.SearchStrategy.DEEP

 DEEP asset searches would search Asset with preference, multiple zonal search and what not 



---
## Type Model.Storage.FileDeleteModel

 Model to delete a file in Storage Service 



---
## Type Model.Storage.DeletionStatus

 Represents status of file deletion from storage service 



---
## Type Model.Storage.FileDownloadModel

 Model to download a file from a Storage Service 



---
#### Property Model.Storage.FileDownloadModel.BlobStream

 Blob stream of the file 



---
#### Property Model.Storage.FileDownloadModel.BlobFileName

 Filename that is used to save the file in the blob 



---
#### Property Model.Storage.FileDownloadModel.BlobContentType

 Content Type of the blob 



---
#### Property Model.Storage.FileDownloadModel.BlobLength

 Blob Length 



---
## Type Model.Storage.FileUploadModel

 Model to upload files to a Storage Service 



---
#### Property Model.Storage.FileUploadModel.FileName

 Desired File Name 



---
#### Property Model.Storage.FileUploadModel.FileUrl

 Url the file is being uploaded from 



---
#### Property Model.Storage.FileUploadModel.FileSizeInBytes

 File Size in Bytes 



---
#### Property Model.Storage.FileUploadModel.FileSizeInKb

 File Size in Kilobytes 



---
#### Property Model.Storage.FileUploadModel.FileSizeInMb

 File Size in Megabytes 



---
#### Method Program.Main

 This is the entry point of the service host process. 



---


