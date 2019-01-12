Front-End and WebApi are 2 separate applications as they need 2 different sets of development tools
To run Front End you need to navigate to MVCAngularTest.FrontEnd folder  using command line and run 2 commands
npm install
ng serve

Prior to running ng serve you will need to start MVCAngularTest.WebApi by running it VisualStudio - 
by default it will run on IIS express - http://localhost:60737/
If you like to use IIS then front-end variable needs to be updated in api-urls/ts to point to your WebApi urls


Dependency injection implemented using Unity
Data Access implemented using EntityFramework
Logging via Serilog
Front-End implemented using Angular 6 based on Angular CLI

