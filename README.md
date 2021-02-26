# Mage

The Mage suite of tools provide access to DMS files and metadata.

## Mage Extractor

Mage Extractor can extract MS/MS search results from MS-GF+, X!Tandem, 
Inspect, or SEQUEST analysis jobs and combine the results into a single 
tab-delimited text file or a single SQLite database.

To use Mage Extractor, first use one of the top tabs to search for 
a series of analysis jobs.  For example, you could search for 
Datasets that contain `qc_shew_2d_11_01_frac` and were searched
with analysis tool "tandem".

Next, change "Result Type to Extract" to the appropriate mode given
the analysis jobs you searched for.  Next define a destination 
file (or SQLite database).  Next, choose the desired filters and/or 
MSGF cutoff, then click either "Extract Results From Selected Jobs" or 
"Extract Results From All Jobs"

Results will be read from the specified jobs, filtered, and
combined into a single, tab-delimited output file.

### Illustrated Example

The following graphic illustrates the steps required to use the Flex Query tab of Mage Extractor to select a series of analysis jobs and obtain the data
* Define filters and click "Get Jobs"
* Define the destination directory and file
* Choose the file type to process
* Choose a filter set (optional) or MSGF cutoff (optional)
* Click the desired "Extract Results" button

![Mage Extractor Directions](https://github.com/PNNL-Comp-Mass-Spec/Mage/blob/master/Docs/Mage_Extractor_StepByStep_Directions.png)

## Mage File Processor

Mage File Processor can be used to copy files from a series of 
analysis job or dataset directories to your local computer.  The program
can either copy each file individually, or it can combine a series of
files into a single file.

An alternative mode of operation is to search a local directory on
your computer to find matching files, then either copy those files
to a new directory, or process those files to combine the data into a
single file.

When combining MS-GF+, X!Tandem, or SEQUEST result files together, you can 
optionally filter the data using one of the standard DMS Filter Sets.
Another useful feature is to automatically add the Job column as the
first column in each row of data in the combined file.

### Illustrated Example

The following graphic illustrates the steps required to use the Flex Query tab of Mage File Processor 
to select a series of analysis jobs and obtain the associated _isos.csv files
* Define filters and click "Get Jobs"
* Define the filename filter, for example: *_Isos.csv
* Customize the search options
* Whether or not to search subdirectories
* Search for files, directories, or both (typically search for files)
* Whether to use RegEx matching or "File Search" matching
* Note that "File Search" matching supports an asterisk as a wildcard symbol: *
* Define the destination directory
* Optionally apply a prefix to each file, for example the Job number

![Mage File Processor Directions](https://github.com/PNNL-Comp-Mass-Spec/Mage/blob/master/Docs/Mage_FileProcessor_StepByStep_Directions.png)

## Mage Metadata Processor

Queries DMS to lookup metadata or stats on the specified datasets.

## Ranger

Iterates through a range of values to create all possible combinations of the values.

## Contacts

Written by Gary Kiebel and Matthew Monroe for the Department of Energy (PNNL, Richland, WA) \
E-mail: proteomics@pnnl.gov \
Website: https://omics.pnl.gov or https://panomics.pnl.gov

## License

Mage is licensed under the 2-Clause BSD License; 
you may not use this program except in compliance with the License.  You may obtain 
a copy of the License at https://opensource.org/licenses/BSD-2-Clause

