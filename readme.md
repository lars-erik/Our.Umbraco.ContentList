# Our Umbraco Content List

## Overview

An responsive content list component for the Umbraco grid.
Features themable views and extensible data sources.

## Installation

`install-package Our.Umbraco.Community.ContentList`

## Requirements

The list component is built around an interface partially derived from `IPublishedContent`.  
It is meant to be used for _any_ content, not only Umbraco documents.  
However, since most of the properties are called the same as `IPublishedContent` properties,
implementing it is in the best case a matter of "tagging" a composition.

In order to "tag" a composition the site must use ModelsBuilder, and there must be 
[non-generated C# files complementing the generated models](https://our.umbraco.com/documentation/reference/templating/modelsbuilder/Builder-Modes).  
The recommended mode for content list with ModelsBuilder is `AppData` mode and manual builds.

## Using a composition

A flexible way to use the content list component is to [create a composition](https://our.umbraco.com/documentation/Getting-Started/Data/Defining-content/) 
for the properties that will be used in lists.

Let's say we call it "Listable Content":



