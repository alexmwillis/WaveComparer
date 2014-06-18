Feature: Compare Audio Files
	Since comparing audio files is very subjective, the tests below set out to 
	solve the more particular problem of comparing drum samples. Here comparing 
	means using a distance measure to order audio files. 

Scenario: The distance between two bass kicks should be less than the distance between a kick and a snare
	Given two kickdrums and a snare
	When when these samples are compared 
	Then the distance between the two kick drums should be less than the distance between the kick drums and the snare
