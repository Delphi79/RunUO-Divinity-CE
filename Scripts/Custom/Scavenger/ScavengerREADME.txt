//*********************************************************************
//*	Scavenger Hunt File: ScavengerREADME.txt
//*
//*	Author: FrayedString
//*
//*	Description: User guide for the Scavenger Hunt scripts.
//*	
//*	Release: 3.2
//*
//*	Scavenger Hunt includes the following files:
//*		- ScavengerBasket.cs		- ScavengerCmd.cs
//*		- ScavengerItemCounter.cs	- ScangerItems.cs
//*		- ScavengerSignup.cs		- ScavengerSignupGump.cs
//*     	- ScavengerREADME.txt           - ScavengerLicense.txt
//*     	- ScavengerCHANGELOG.txt
//*
//*
//*  This document is intended as a brief overview of *what* the scavenger
//*  hunt system is, as well as basic directions for it's use on RunUO freeshards.
//*
//*  This system is a basic framework, which I hope to improve upon over time, to
//*  allow game staff to host Scavenger Hunt events for their players.  In order
//*  to begin hosting events, a Game Master will have to add several items to the world.
//*  The first is a ScavengerSignup, this large signup stone is crucial to the event
//*  because it is the only way the players can aquire the ScavengerBasket, which is
//*  an essential tool for the scavenger hunt.
//*
//*  The ScavengerSignup has 1 property of note which is 'signupEnabled'.
//*  This boolean (true/false) value determines whether or not the stone will respond to a player's
//*  double click request to be signed up for the event.  This property may be left enabled through
//*  the entire duration of the event, so if staff chooses to allow it, players may sign up even
//*  after the event is well under way.
//*
//*  The ScavengerSignup also has properties relating to the amount of money paid to the participants.
//*
//*  It is also imperative to add some ScavengerItem (the items the players will be searching for)
//*  to the world, as the actual hunt cannot be started until there are some items present in the world.
//*  The visibility of a ScavengerItem upon it's initial creation is determined by the current
//*  status of the scavenger hunt.  If the scavenger hunt start command has not yet been issued
//*  the items are added invisible, so as to not give away their locations prior to the event's
//*  beginning.  Similarly, if the item is added after the start command has been issued, the
//*  items will be added visible for immediate collection.  This is to allow game staff to
//*  add items as the event progresses if they are being found too quickly.
//*
//*  There is also an item called the ScavengerItemCounter, which works much as it's name suggests.
//*  This item is intended for use by the staff running the event in order to determine
//*  whether or not to add more items or go ahead and end the event.
//*
//*  The scavenger hunt system includes 2 commands.  They are [startscavengerhunt
//*  and [stopscavengerhunt.  While their purpose is fairly apparent from their names
//*  I will briefly touch on the conditions and purposes of both of them.
//*  The first command that you will need to issue when running a scavenger hunt
//*  is [startscavengerhunt.  This command will only execute if the following conditions are met.
//*
//*  1) There is at least 1 ScavengerItem present in the world. 
//*  2) There are at least 3 players signed up for the event.
//*
//*  This command will reveal all of the pre-placed scavenger items and prompt the players to begin
//*  searching for them.
//*
//*  The second command associated with this system is [stopscavengerhunt.  It is the
//*  hosting staff's decision as to when to issue this command.  It can be used at any point
//*  after the [startscavengerhunt command.  This command will remove any remaining unfound
//*  scavenger items, issue payouts to the participants based on their ranking and number
//*  of items found, and remove all scavenger baskets that were used.  Thus cleaning up the world
//*  and preparing everything for the next scavenger hunt! :D
//*
//*
//*  Special Thanks:
//*
//*  ASayre - you rock.
//*  Mia - for the inspiration.
//*
//*********************************************************************