GAFAWS is a .Net (C#) implementation of [Joseph E. Grimes'](http://www.sil.org/resources/search/contributor/grimes-joseph-e) 1983 work entitled [Affix Positions and Cooccurrences: The PARADIGM Program](http://www.sil.org/resources/archives/8825). Grimes' original implementation was written in [PTP](http://www.sil.org/computing/catalog/show_software.asp?id=39).

# Status
The core GAFAWS code has been used by the parser in FieldWorks for a number of years. There is a GUI interface that allows users to convert data from various formats, so it can be fed into the core system for analysis. The results of the analysis are then displayed in an internet browser.

Grimes' book describes two kinds of analysis: 1) positional, and 2) co-occurrences. (See [Part One: Morphology](http://www.sil.org/system/files/reapdata/28/03/36/28033685206467803523518673099687647776/18632.pdf) of the book for details. Requires [Adobe Reader](http://www.adobe.com/products/reader/)) Positional, co-occurrence, and basic component subgraph analyses are currently supported.

See: [LingTranSoft](http://www.lingtransoft.info/apps/gafaws-position-analyzer) to review the program or vote on it.

# Supported input formats
* [Ample's](http://www.sil.org/computing/catalog/show_software.asp?id=1) ANA output
* A specially formatted plain text file
* [FieldWorks (7.0, or higher)](http://fieldworks.sil.org/download) XML file

Download version 2.0 RC1 [here](http://projects.palaso.org/attachments/download/71/GAFAWS2.0RC1.zip).

# Technical
Released version requires [.Net 3.5](http://www.microsoft.com/downloads/details.aspx?FamilyID=333325fd-ae52-4e35-b531-508d977d32a6&DisplayLang=en).

Developer version requires [.Net 4.0](http://www.microsoft.com/en-us/download/details.aspx?id=17718).

To get the code, install Git, and do

git clone https://github.com/sillsdev/gafaws.git