﻿Feature: Manage Customers via REST API

@RestApi
Scenario: Create a new customer via REST
	Given there are no customers named 'Holly Henshaw'

	When I create a new customer resource with the following details
	| Title | Name          | Address Line 1 | Address Line 2 | Address Line 3 | Postcode | Home Phone   | Mobile       | Account Invoicing |
	| Miss  | Holly Henshaw | 101 Highfields | Duston         | Northampton    | NN5 6HH  | 01604 789456 | 07777 987654 | No                |

	Then I should receive an HTTP 201 Created response
	And I should receive the location of the created resource
	And the response should be in JSON format
	And the customer should be added to the system with the details provided

@RestApi
Scenario: Retrieve a customer via REST
	Given this existing customer
	| Title | Name          | Address Line 1 | Address Line 2 | Address Line 3 | Postcode | Home Phone   | Mobile       | Account Invoicing |
	| Miss  | Holly Henshaw | 101 Highfields | Duston         | Northampton    | NN5 6HH  | 01604 789456 | 07777 987654 | No                |

	When I request the customer resource

	Then I should receive an HTTP 200 OK response
	And the response should be in JSON format
	And I should receive the full details of the customer

@RestApi
Scenario: Update a customer via REST
	Given this existing customer
	| Title | Name          | Address Line 1 | Address Line 2 | Address Line 3 | Postcode | Home Phone   | Mobile       | Account Invoicing |
	| Miss  | Holly Henshaw | 101 Highfields | Duston         | Northampton    | NN5 6HH  | 01604 789456 | 07777 987654 | No                |

	When I update the customer resource with the following changes
	| Mobile       | Account Invoicing |
	| 07771 123456 | Yes               |

	Then I should receive an HTTP 204 No Content response
	And the changes should be made to the customer in the system

@RestApi
Scenario: Delete a customer via REST
	Given this existing customer
	| Title | Name          |
	| Miss  | Holly Henshaw |

	When I delete the customer resource

	Then I should receive an HTTP 204 No Content response
	And the customer should be removed from the system

@RestApi
Scenario: Attempting to retrieve a non existent customer via REST returns Not Found
	Given there is no customer with ID 100

	When I request a customer resource with ID 100

	Then I should receive an HTTP 404 Not Found response