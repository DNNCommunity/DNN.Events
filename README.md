# DNN.Events
The Events module is part of the standard core modules of DNN, the web content management system and web application framework. (DotNetNuke®)DNN Events manages display of upcoming events as a list in chronological order or in calendar format with additional information.
 
 Please view the [Wiki](https://github.com/DNNCommunity/DNN.Events/wiki) for full documentation  

  * Install and source packages can be downloaded from [the releases page on GitHub](https://github.com/DNNCommunity/DNN.Events/releases).
  * Feature requests, bug reports and general questions can be submitted via [the issues page on GitHub](https://github.com/DNNCommunity/DNN.Events/issues).  
  * More Information about our [move to GitHub](https://github.com/DNNCommunity/home/wiki).  
  
**Note**: For the latest version of the DNN Events source code, fork the project to your own GitHub repository and start contributing today!  

# Getting started

  * [Download and install](https://github.com/dnnsoftware/Dnn.Platform/releases/) (if you have not done already) DNN CMS.  
  * [Download and install](https://github.com/DNNCommunity/DNN.Events/releases/) the latest release of Events on your site    
  * Start with experimenting!  
  
# Suggestions? Features? Questions?  

  * Visit the [Module forum](https://dnncommunity.org/forums/aff/8) on DNNCommunity.org
  * Add bugs or feature requests to the [Issue Tracker](https://github.com/dnncommunity/dnn.events/issues). Help us shape the Events module with your feedback!  
  
# Contributions
If you would like to contribute to this repository, please read the [CONTRIBUTING.md](https://github.com/DNNCommunity/DNN.Events/blob/master/.github/CONTRIBUTING.md)

# An overview of functions and possibilities of the Events module  

## Various views  

The Events module support various way of displaying events, all handled by selectable CSS based themes that can be created and modified.  

Calendar Month View|List View
-------------------|---------  
![DNNEvents](https://raw.githubusercontent.com/wiki/DNNCommunity/DNN.Events/DNNEvents.png)|![List View](https://raw.githubusercontent.com/wiki/DNNCommunity/DNN.Events/List%20View.png)  
**Week View**|**Detail View**    
![Week View](https://raw.githubusercontent.com/wiki/DNNCommunity/DNN.Events/Week%20View.png)|![Detail View](https://raw.githubusercontent.com/wiki/DNNCommunity/DNN.Events/Detail%20View.png)  

See [Event views](https://github.com/DNNCommunity/DNN.Events/wiki/Event-Views) for more information.

# Various types of Events  

Different types of events can be entered using event settings to specifiy details. The module can prevent time conflicts and/or location conficts of different events.  

  * Single Event.  
  * Recurring Events with period until per day, week, day of week, month, day of month, every 1st etc of month, annual.  
  * Whole day events, events with or without end time, multi day spanning events.  
  * Edit series in once edit individual entries from a serie, delete individual entries from a serie.  
  * Copy existing event to new event.  
  * Copy recurring events to new series of Events.  
  * With and /or without image display.  

# Security settings with workflow support  

Event edit and an approval rights, maintenance of standard tables like category and location can be assigned to different roles and/or users.

  * Edit Locations  
  * Edit Categories  
  * Add & Approve Events  
  * Edit & Approve enrollment  

# Optional enrollments for Events  

If specified, visitors can enroll for events with various validations and settings.

  * Enrollment for registered users  
  * Enrollment approval cycle or automatic enrollment  
  * Enrollment with limited number of possible attendees  
  * Free Enrollment of payed (PayPal) enrollment  
  * Enrollment of a specified number of people  

# Notifications    

Users can be notified by e-mail on events related to events:  

  *  New Event created  
  * A reminder in a selected time before the Event  
  *  New users enrolled  
  * Users approved and/or denied for enrollment  

# Events themes  

The Events module supports themes (skins): standard themes are installed automatically, custom themes can be created and uploaded by the user.  

Events theme|Blue theme  
------------|----------  
![DNNEvents2](https://raw.githubusercontent.com/wiki/DNNCommunity/DNN.Events/DNNEvents2.png)|![Event Themes2](https://raw.githubusercontent.com/wiki/DNNCommunity/DNN.Events/EventThemes2.png)
**Grey theme**|**Small theme**  
![Event Themes3](https://raw.githubusercontent.com/wiki/DNNCommunity/DNN.Events/EventThemes3.png)|![Event Themes4](https://raw.githubusercontent.com/wiki/DNNCommunity/DNN.Events/EventThemes4.png)  


See [Event themes](https://github.com/DNNCommunity/DNN.Events/wiki/Event-Themes) for more samples of Event themes  

# Related pages  

[Home](https://github.com/DNNCommunity/DNN.Events/blob/development/README.md)  
[Event views](https://github.com/DNNCommunity/DNN.Events/wiki/Event-Views)  
[Event settings](https://github.com/DNNCommunity/DNN.Events/wiki/Event-Settings)  
[Event themes](https://github.com/DNNCommunity/DNN.Events/wiki/Event-Themes)  
[Templating Event views](https://github.com/DNNCommunity/DNN.Events/wiki/Templating-Event-Themes)  


#Supported by

[![ReSharper](https://raw.githubusercontent.com/wiki/DNNCommunity/DNN.Events/ReSharper%20Support.png)](https://www.jetbrains.com/resharper/)  

## Maintainers
This module is currently maintained by Ernst Peter Tamminga (@EPTamminga)
Please coordinate with him before publishing any new release and ask his review on any pull request.
