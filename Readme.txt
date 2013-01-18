The Mage suite of tools provide access to DMS files and metadata.


== Mage Extractor ==

Mage Extractor can extract MS/MS search results from SEQUEST, X!Tandem, 
Inspect, or MSGF+ analysis jobs and combine the results into a single 
tab-delimited text file or a single SQLite database. MAGE Extractor is 
the successor to STARSuite Extractor; it includes support for MSGF results.

To use Mage Extractor, first use one of the top tabs to search for 
a series of analysis jobs.  For example, you could search for 
Datasets that contain "qc_shew_2d_11_01_frac" and were searched
with analysis tool "tandem".

Next, change "Result Type to Extract" to the appropriate mode given
the analysis jobs you searched for.  Next define a destination 
file (or SQLite database).  Next, choose the desired filters and/or 
MSGF cutoff, then click either "Extract Results From Selected Jobs" or 
"Extract Results From All Jobs"

Results will be read from the specified jobs, filtered, and
combined into a single, tab-delimited output file.


== Mage File Processor ==

Mage File Processor can be used to copy files from a series of 
analysis job or dataset folders to your local computer.  The program
can either copy each file individually, or it can combine a series of
files into a single file.  

An alternative mode of operation is to search a local folder on
your computer to find matching files, then either copy those files
to a new folder, or process those files to combine the data into a
single file.

When combining Sequest or X!Tandem result files together, you can 
optionally filter the data using one of the standard DMS Filter Sets.
Another useful feature is to automatically add the Job column as the
first column in each row of data in the combined file.


== Mage Metadata Processor ==

Queries DMS to lookup metadata or stats on the specified datasets.


== Ranger ==

Iterates through a range of values to create all possible combinations of the values.


-------------------------------------------------------------------------------
Written by Gary Kiebel and Matthew Monroe for the Department of Energy (PNNL, Richland, WA)
Copyright 2011, Battelle Memorial Institute.  All Rights Reserved.

E-mail: matthew.monroe@pnnl.gov or matt@alchemistmatt.com
Website: http://ncrr.pnnl.gov/ or http://omics.pnl.gov
-------------------------------------------------------------------------------

