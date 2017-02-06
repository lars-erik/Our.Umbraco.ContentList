Feature: UA01 Set up a content list
	As an editor
	In order to show lists of content
	I want to configure a listing in the grid

Background:
    Given I am logged in to the backoffice

Scenario: US01-01 Add a content list
    And I create a new TextPage
    When I add a Content List
    Then a content list editor is added
    And the preview shows "Please select data source"

Scenario: US01-02 Start configuring a content list
	Given I have added a content list to a new page
    When I click the preview
    Then the settings dialog is shown

