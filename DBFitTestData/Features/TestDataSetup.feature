Feature: TestDataSetup
	In order to have repeatable test
	I want to setup-teardown test data for each scenario

@dbFit
Scenario: Setup-Teardown test data
	Given Any Scenarrio
	Then Test data should be inserted then rolled back
