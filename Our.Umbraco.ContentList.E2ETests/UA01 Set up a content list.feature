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
    And the preview shows "Please select a data source"

Scenario: US01-02 Start configuring a content list through settings
	Given I have added a content list to a new page
    When I click the preview
    Then the settings dialog is shown

Scenario: US01-03 Configuring a content list with the wizard
	Given I have added a content list to a new page
    Then the wizard prompts me to "Select a data source"
    When I select the "XPath Query" data source
    And I click "Next"
    Then the wizard prompts me to "Enter query parameters"
    When I enter "//BlogPost"
    Then I see the message "3 items returned by this query"
    When I click "Next"
    Then the wizard prompts me to "Select a template"
    When I select the "Default" template
    And I click "Next"
    Then a preview of 3 items is displayed

Scenario: US01-02-02 Being prompted for data source parameters
	Given I have added a content list to a new page
    And I have opened the settings
    When I select the "XPath Query" data source
    And I click "Submit"
    Then the preview shows "Please set up the query parameters"

