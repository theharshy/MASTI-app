# MASTI - Messenger Application for Student-Teacher Interaction.

An application that facilitates interaction between multiple students and a teacher.     

* Live screen-sharing support included.
* Facilitates texting between student and teacher. 

# Repository Structure

The folders corresponding to each module contains _(should contain)_:

- A folder `Specs` contains the specs of the team leader and team members.
- The team leader's spec is titled `ReadMe.md`.
- A folder for each Markdown file, which contains the images used in the file. 
- Other files and folders concerning the code.  


# Authorization:

- Team leads will fork this repository and send pull requests. 
- Pull requests to be entertained _only_ if they're from the team leads. 
- Architect to be added as a collaborator. 

# Evaluation methodology

- Everything will be evaluated on the BitBucket repository, no new submission portals will be created. 
- Ram sir will be given write access to the repository, so that he could evaluate and suggest edits. 

# Standardisation norms

## Documentation

- All design documents _must_ be in markdown format. 
- All design specs _must_ be in the **Specs** folder. 
- Design specs include the specs of Team Leads and individual team members. 

### Team Lead's document:

- Components that you will depend on, and the components that in turn will depend on. 
- The interfaces you are going to expose/consume (some pseudo code). 
- The various internal components involved, the developers for each of the component and also any test code developers. 
- High level class diagram, activity diagram. 

### Individual member's document:

- Details for each internal component you are working on. 
- The classes involved etc. 

## UML Diagrams
### Class Diagram
- Team lead must submit a UML diagram of all the internal class dependencies within the concerned module. _(This is needed for the Architect's master UML diagram.)_ 
### Module Diagram
- A module diagram shows dependencies of your module on rest of the modules. 
### Activity Diagram
- Activity diagram is like a flow-chart which would indicate how the control flows through your module. 

## Naming Files and Folders

- A folder `Specs` contains the specs of the team leader and team members.
- The team leader's spec is titled `ReadMe.md` and is located in `Specs` folder. 
- All the images used in `ReadMe.md` to go in folder `Specs/ReadMe`. 
- Team members' specs would be titled `111501001Firstname.md` and to be located in `Specs` folder.
- All the images used in `111501001Firstname.md` to go in folder `Specs/111501001Firstname`. 
- Other files and folders concerning the code.  

## Coding Standards
* Enforced using StyleCop and FxCop. 
* Files could be found in Templates folder

### Header format:
```Csharp
//-----------------------------------------------------------------------
// <author> 
//     Author's full name 
// </author>
//
// <date> 
//     dd-mm-yyyy 
// </date>
// 
// <reviewer> 
//     Reviewer's name 
// </reviewer>
// 
// <copyright file="ICommunication.cs" company="B'15, IIT Palakkad">
//    This project is licensed under GNU General Public License v3. (https://fsf.org)
// </copyright>
// 
// <summary>
//		A brief description of the file.
// </summary>
//-----------------------------------------------------------------------
```
--------------------------------------------------
