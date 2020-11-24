# API.TestAssignment

Hello there! 

This is a coding assignment which is very similar with our projects in Linkfire. We are working on aggregating information from different media services (such as Spotify, Deezer, iTunes, etc..) under one URL. Check this to see how a smart link looks like.

- https://lnk.to/test_link - music link landing page
- https://tix.to/test_link - ticket link landing page

There you'll see various media services (destinations), where the user has the ability to set up different destinations per country. The list of the destinations can be different depending on which country the landing page has been opened. For example, if some media services are not available in some countries, or the url is different for different countries.

We respect your time, so we prepared a code template of the task below. There is a couple of 'to do' that you could implement. If you have time and prefer to do **everything from scratch, it is also possible**. We are trying to be as flexible as possible and give you the chance to decide on the solution and the effort that you want to put on this task. 

### What will be evaluated and appreciated:

- ability to write clean, readable and supportable code
- ability to recognize where a pattern/principle could be used and the ability to apply this pattern/principle
- ability to work with existing code (if working with the template will be chosen)
- ability to organize your code
- ability to write documentation to your own code		

### If you decide to go with the template, we expect from you:

1. to get through the code
2. understand it
3. finish some 'to do', entire list could be looked up in 'Task List' (see https://docs.microsoft.com/en-us/visualstudio/ide/using-the-task-list?view=vs-2017) (*optional*)
4. think about the case when multiple users are updating the same link, how to avoid DB entity and Storage model be out of sync (*optional*)
5. refactoring the code, for example, there is a lot of code duplication (*optional*)

**We are not expecting to solve ALL the problems**, peek a couple of them and write why you choose them in your email, you could also name other problems, and it would be nice to have, a short description of how to solve them.

Example:
```
Fixed:
Code duplication | Fixed that, as I think, it will help to support code in feature.
Noted:
Not all part of the code are testable | Inversion of Control can be implemented (+ name actual places where you found this, 1-2 will be enough)
```

## The Goal

Implement [CRUD](https://en.wikipedia.org/wiki/Create,_read,_update_and_delete) operation for links with WEB.API. Information that could be used for link search should be in DB (such as Title, Artist, Code, Domain). All other information (destination and tracking info) should be stored in a local file, assuming that this file will be accessible by another system by domain/code. 

Let's try to understand what is Link entity and what relations it should have.

#### Link:

- Title (required, max length: 255)
- Code  (min length: 2; max length: 100, should be alphanumeric, special characters are not allowed, except underscore) - if not provided random code should be generated 
- URL   (required, rule for URLs should be applied)
- MediaType (required, indicate if it Ticket or Music link)
- Domain  (required, assume that there is another source of available domains, to current DB replicated only Name and Id, so for this task it is static data)
- Artists (list of artists, used as a 'tag' for search)
- Link could not be removed, only marked as deleted
- **Music Link** should have 
  	* a list of Music.Destinations per country(ISO code) (could be ~ 1000 per Link)
  	* TrackingInfo - initial info(like song title, album) about link on particular media service. NOTE: assume that it is populated after link creation, should not be ever overwritten with API 
- **Ticket Link** should have a list of Ticket.Destinations per country(ISO code) (could be < 100 per Link)
		
#### Artist:		
- Name  (required, max length: 255)
- Label (max length: 255)
		
#### Music.Destination:
- MediaServiceId
- TrackingInfo
		
#### Ticket.Destination:
- ShowId   (required) - unique identifier of ticket destination
- MediaServiceId (required)
- URL		 (required, rule for URLs should be applied)
- Date	 (required)
- Venue    (required, max length: 255)
- Location (required, max length: 255)
- ExternalId - string - should not be exposed to API, assume that it is being set from another system
		
#### TrackingInfo:
- MediaServiceName - string (MediaService.Name) - should not be exposed to API, being set on internally from media service by MediaService.Id
- Mobile		(rule for URLs should be applied) - URL for redirecting users if they use mobile
- Web			(required, rule for URLs should be applied) - URL for redirecting users if they use mobile
- Artist		- string
- Album		- string
- SongTitle	- string 

#### MediaService (predefined):
- Name (required, max length: 255)

#### Domain (predefined):
- Name (required, max length: 255)

### Acceptance criterions (validation)

link creation: 
- argument exception should be thrown in case if combination of *domain/code* is used. Also if it is conflict with reserved *domain/code*. **Reserved shortlinks** are all possible combinations for current shortlink plus 2 symbols or changed 2 last symbols, i.e.. for shortlink *domain/code* - *domain/co{+2 symbols}* should be reserved, as well as *domain/code{+2 symbols}*
- music destinations with nonexistent media services should not be saved

link updation:
- if domain or code were changed same validation should be applied
- update of MediaType should not be supported
- if link is not active or not exists exception should be thrown
- music destinations with nonexistent media services should not be saved
- ExternalId for ticket destinations should not be updatable

link deletion:
- link should not be physically removed, but shortlink should be available for other link
- link files should be moved to *domain/{linkId}*
- if trying to remove inactive link exception should be thrown

## Getting Started

Software versions used:

  .NET Framework 4.6.1;
  Microsoft SQL Server Express 13.0.4001 SP1;
 
  Visual Studio 2017;
  Microsoft SQL Server Management Studio v17.6;
