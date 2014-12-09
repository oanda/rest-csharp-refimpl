rest-csharp-refimpl
===================

Reference implementation of the OANDA REST API in C#

Refer to developer.oanda.com for full details and documentation of the API

## Overview

This solution contains three projects:

1. OANDARestLibrary
  * This is the central library or wrapper for the API
2. ConsoleTest
  * Console based test library that runs through many useful scenarios with the rest library
  * If you're looking for how to do something (eg. modify a trade, stream rates, delete a position) this is a good place to start
3. TradingApp2
  * Windows 8 App demonstrating some of the functionality of the API in a visual application
  * Also demonstrates the client side OAuth flow

## How to get started

* Open the solution in visual studio (developed with VS2012 pro, but more recent versions should work)
* Set TradingApp2 as the startup project
* Run
* When prompted, login with your practice account

* Set ConsoleTest as the startup project
* Edit RestTest.cs, search for SetCredentials, and input your credentials for it to use
 * Note: This test sequence will automatically place trades using the specified credentials
* Run

## OANDARestLibrary Overview

* Primary file is Rest.cs
 * This is (nearly) the only file you should need to call into directly
 * Contains functions for all actions that can be taken with the API
 * Automatically refers to Credentials for access information (see below)
* Credentials.cs
 * Used to set the credentials for the library to use (SetCredentials)
 * Stores the server information for all the API environments
* Framework
 * Some supporting framework classes
* TradeLibrary
 * RatesSession - used to create a new streaming rates session
 * EventsSession - used to create a new streaming events session
 * DataTypes - various data types used for representing the data returned from the API
 * DataTypes\Communications - extra classes used for communicating to and from the API, mostly just for the internal use of the library
 * 
 
## Warning
Leverage trading is high risk and not suitable for all. You could lose all of your deposited funds. Articles are for general information purposes only and are not investment advice or a solution to buy or sell any investment product. Opinions are those of the authors and not necessarily those of OANDA, its officers, or its directors. Examples shown are for illustrative purposes only and may not reflect current prices or offers from OANDA


