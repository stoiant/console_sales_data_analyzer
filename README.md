# Sales Data Analyzer

##Deliverable

Zip containing all conde and a README.txt containing:

	* steps to build and run the program
	* interesting trade-offs or edge cases you ran into
	* any important choices made because of time
	* anything else you'd like us to know


## Business Requirements

Write a command line tool that takes in these two files as arguments on the command line (the SalesData.csv file and ProductData.json file) and prints to standard out the five product types with the best peak / non-peak sales ratio.

That is, for each product type, we want to find the number of sales during the peak period (which we'll define here as "all orders placed in October, November and December") divided by the number of sales during the non-peak period (orders placed in any other month), and display the five product types with the highest result.  E.g.:

```
small\_furnishing: 2.9889
organizational: 1.002
kitchen: 0.51
bedroom\_essentials: 0.23
desk\_accessories: 0.127
```

###Input Files

####Sales Data

This is a file containing a record of sales made on wayfair's storefront.  It looks like this:

```
product\_id,order\_date,ship\_date,sale\_price\_in\_cents,nps_score
CRT1976,2017-10-31,2017-11-02,7899,7
BTL2327,2017-10-01,2017-11-15,1000,2
```

####Product Data

This is a json file containing information about products.  It looks like this:

```
[
{'product_id': ‘CRT1976’,
 'product_name': 'Red lamp with golden leaf trim',
 'product_type': 'small_furnishing',
 'product_class': 'sp',
 'packaging': 'standard'},

{'product_id': ‘BTL2327’,
 'product_name': 'Reed box',
 'product_type': 'organizational',
 'product_class': 'sp',
 'packaging': 'light_overpack'}
]
```


## Installation

### Running Requirements

* Windows
* .NET 4.6.1 installed `LINK TO DOWNLOAD`
* Sufficient Space
* Sufficient Ram

### Steps to setup environment

* VisualStudio 2017 or later
* NuGet
* .NET 4.6.1 installed `LINK TO DOWNLOAD`

## How to Run the tool

`DataAnalyzer.exe --help `
* Provides usage help

`DataAnalyzer.exe -s sales_data.json -p product_data.json `

To run the provided input if everything is in the same folder.

## Logging
Basic logging is available and configurable.

## Return Codes
0 - Upon Success
2 - Upon Failure

## Limitations

* invalid input is ignored
* division by 0 is ignored
* limited testing performed
* limited coverage due to time

## Testing

### Dev Environment

* Windows 10
* IDE (VisualStudio 2017 Community Edition)

### Performance Testing
Processing large >1GB file quite fast, no time to perform timing.

### Large Input Testing
Processing large >1GB file without problem.
### Invalid Input Testing
Partially completed no time for complete suite.

### Provided Examples
Unable to check if working as there was no expected output provided.
 
### General Testing
Mostly manual, no time to automate.

### Testing Wishlist
Too much to list :)

## Framework Dependencies

* Newtonsoft.json	
* ManyConsole
* Log4Net

## Tool Wishlist
* .NET Core implementation to allow running on Linux
* Multithreaded reading of input to maximize performance for large files
* Parsing of CSV using a framework such as CSVHelper to allow fro object mapping and full CSV support
* More test coverage

