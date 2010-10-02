<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">
  <xsl:output method="html" version="1.0" encoding="UTF-8" indent="yes"/>
  <!--
================================================================
Convert xml file conforming to GAFAWS.xsd to an html page showing the chart
  Input:    xml file conforming to GAFAWS.xsd
  Output: HTML file showing the chart
================================================================
Revision History is at the end of this file.

- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Preamble
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
  <!-- Some global variables -->
  <!-- number of prefix and suffix columns -->
  <xsl:variable name="iPrefixColumns" select="count(//PrefixClasses/Class)"/>
  <xsl:variable name="iSuffixColumns" select="count(//SuffixClasses/Class)"/>
  <!-- non-breaking space -->
  <!--  <xsl:variable name="sNonBreakingSpace">&#xa0;</xsl:variable> -->
  <xsl:variable name="sNonBreakingSpace"/>
  <!-- Error message for cells that do not compute -->
  <!-- <xsl:variable name="sError"><xsl:value-of select="$sNonBreakingSpace"/></xsl:variable> -->
  <xsl:variable name="sError">???</xsl:variable>
  <!-- indentation for several items -->
  <xsl:variable name="sIndent">margin-left:.25in</xsl:variable>
  <!-- background color for cells with data -->
  <xsl:variable name="sBackColorAllGood">background-color:linen</xsl:variable>
  <xsl:variable name="sBackColorAllBad">background-color:lightsalmon</xsl:variable>
  <xsl:variable name="sBackColorEndIsBad">background-color:LightBlue</xsl:variable>
  <xsl:variable name="sBackColorSpansBad">background-color:lightgreen</xsl:variable>
  <xsl:variable name="sBackColorAffix">background-color:yellow</xsl:variable>
  <!-- section number of the data section -->
	<xsl:variable name="sDataSectionNumber">
		<xsl:choose>
			<xsl:when test="count(//Challenge) > 0">5</xsl:when>
			<xsl:otherwise>4</xsl:otherwise>
		</xsl:choose>
	</xsl:variable>
  <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Main template
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
  <xsl:template match="GAFAWSData">
	<html>
	  <head>
		<title>Affix Position Chart</title>
	  </head>
	  <body>
		<p align="center" style="font-weight:bold; font-size:250%">Affix Position Chart Results</p>
		<p align="center">Results of: <xsl:value-of select="@date"/> at: <xsl:value-of select="@time"/></p>
		<xsl:apply-templates select="Classes"/>
		  <xsl:apply-templates select="AffixSets" />
		  <xsl:apply-templates select="SubgraphSets" />
		<xsl:apply-templates select="Challenges"/>
		<h1>
		  <xsl:value-of select="$sDataSectionNumber"/> The Data</h1>
		<p>These results are based on the following data items.</p>
		<xsl:apply-templates select="WordRecords"/>
		<xsl:apply-templates select="Morphemes"/>
	  </body>
	</html>
  </xsl:template>
	<!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Elemental Subgraph Sets template
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
	<xsl:template match="SubgraphSets" >
		<h1>3. Specifications for component subgraphs</h1>
		<p>The data contained the following elemental subgraph sets:</p>
		<xsl:for-each select="SubgraphSet">
			<p>
				Subgraph for
				<xsl:choose>
					<xsl:when test="@id = 'xxx'">
						<xsl:value-of select="@id"/>
					</xsl:when>
					<xsl:otherwise>
						<a>
							<xsl:attribute name="href">
								#<xsl:value-of select="@id"/>
							</xsl:attribute>
							<xsl:call-template name="OutputAffix"/>
						</a>
					</xsl:otherwise>
				</xsl:choose>:
				<xsl:apply-templates select="Subgraph"/>
			</p>
		</xsl:for-each>
		<p>Please follow the instructions in the book (Chap. 3, pp 26-28) to complete the part that has to be done by hand.</p>
	</xsl:template>
	<xsl:template match="Subgraph" >
		<p>
			<xsl:attribute name="style">
				<xsl:value-of select="$sIndent"/>
			</xsl:attribute>
			{<xsl:for-each select="Morpheme">
				<xsl:if test="position() &gt; 1">
					<xsl:text>, </xsl:text>
				</xsl:if>
				<xsl:choose>
					<xsl:when test="@id = 'xxx'">
						<xsl:value-of select="@id"/>
					</xsl:when>
					<xsl:otherwise>
						<a>
							<xsl:attribute name="href">
								#<xsl:value-of select="@id"/>
							</xsl:attribute>
							<xsl:call-template name="OutputAffix"/>
						</a>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>}
		</p>
	</xsl:template>
	<!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Affix Sets template
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
	<xsl:template match="AffixSets">
		<h1>2. Affix Sets</h1>
		<p>The data contained the following affixes:</p>
		<p>
			<xsl:attribute name="style">
				<xsl:value-of select="$sIndent"/>
			</xsl:attribute>{<xsl:for-each select="../Morphemes/Morpheme[@type='pfx' or @type='sfx']">
				<xsl:if test="position() &gt; 1">
					<xsl:text>, </xsl:text>
				</xsl:if>
				<a>
					<xsl:attribute name="href">
						#<xsl:value-of select="@id"/>
					</xsl:attribute>
					<xsl:call-template name="OutputAffix"/>
				</a>
			</xsl:for-each>}
		</p>
		<h2>2.1. Regular Cooccurrence Sets</h2>
		<p>For each affix in the above set of affixes, the following shows other affixes that can occur with each:</p>
		<xsl:for-each select="AffixCooccurrences/AffixCooccurrenceSet">
			<p>
				<xsl:attribute name="style">
					<xsl:value-of select="$sIndent"/>
				</xsl:attribute>{<xsl:for-each select="Morpheme">
					<xsl:if test="position() &gt; 1">
						<xsl:text>, </xsl:text>
					</xsl:if>
					<a>
						<xsl:attribute name="href">
							#<xsl:value-of select="@id"/>
						</xsl:attribute>
						<xsl:call-template name="OutputAffix"/>
					</a>
				</xsl:for-each>}
			</p>
		</xsl:for-each>
		<h2>2.2. Non-cooccurrence Sets</h2>
		<p>
			For each affix in the above set of affixes, the following shows other affixes that <i>cannot</i> occur with each:
		</p>
		<xsl:for-each select="AffixNonCooccurrences/AffixNonCooccurrenceSet">
			<p>
				<xsl:attribute name="style">
					<xsl:value-of select="$sIndent"/>
				</xsl:attribute>{<xsl:for-each select="Morpheme">
					<xsl:if test="position() &gt; 1">
						<xsl:text>, </xsl:text>
					</xsl:if>
					<a>
						<xsl:attribute name="href">
							#<xsl:value-of select="@id"/>
						</xsl:attribute>
						<xsl:call-template name="OutputAffix"/>
					</a>
				</xsl:for-each>}
			</p>
		</xsl:for-each>
		<h2>2.3. Distinct Sets of Mutually Exclusive Forms</h2>
		<p>Analysis of the sets of affixes in 2.2 gives these distinct sets of affixes:</p>
		<xsl:for-each select="DistinctSets/DistinctSet">
			<p>
				<xsl:attribute name="style">
					<xsl:value-of select="$sIndent"/>
				</xsl:attribute>{<xsl:for-each select="Morpheme">
					<xsl:if test="position() &gt; 1">
						<xsl:text>, </xsl:text>
					</xsl:if>
					<a>
						<xsl:attribute name="href">
							#<xsl:value-of select="@id"/>
						</xsl:attribute>
						<xsl:call-template name="OutputAffix"/>
					</a>
				</xsl:for-each>}
			</p>
		</xsl:for-each>
		<p>
			TODO: See if we can 'borrow' some explanation from
			<a>
				<xsl:attribute name="href">
					<xsl:text>http://www.sil.org/acpub/repository/18632.pdf</xsl:text>
				</xsl:attribute>
				<xsl:text>the book</xsl:text>
			</a> on what a Distinct Set means to the analyst. Also, try to find some free .Net HTML generating widget that can produce the Venn Diagrams of the Distinct Sets.
		</p>
	</xsl:template>
  <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Classes template
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
  <xsl:template match="Classes">
	<h1>1. Affix Position Chart</h1>
	<p>The following table delineates the various positions discovered:</p>
	<table border="1">
	  <xsl:attribute name="style"><xsl:value-of select="$sIndent"/></xsl:attribute>
	  <!--
	  Prefixes first
	   -->
	  <tr>
		<td valign="top">
		  <!-- NOTE: the following code and the code for suffixes are very similar.  I tried to combine them, but with the maing fore-each loops differing in direction of sorting or needing to llok at preceding/following it became complicated.  As a result I've chosen to repeat code here and under suffixes. -->
		  <table border="1">
			<!-- headers -->
			<tr>
			  <xsl:for-each select="PrefixClasses/Class">
				<xsl:sort select="position()" order="descending"/>
				<xsl:choose>
				  <xsl:when test="@isfogbank = '1'">
					<th>
					  <xsl:attribute name="style"><xsl:value-of select="$sBackColorAllBad"/></xsl:attribute>
					  <xsl:value-of select="$sError"/>
					</th>
				  </xsl:when>
				  <xsl:otherwise>
					<th>
					  <xsl:value-of select="concat('-', substring-after(@id, 'PP'))"/>
					</th>
				  </xsl:otherwise>
				</xsl:choose>
			  </xsl:for-each>
			</tr>
			<!-- prefixes in only one column -->
			<tr>
			  <xsl:for-each select="PrefixClasses/Class">
				<xsl:sort select="position()" order="descending"/>
				<xsl:variable name="sCLID">
				  <xsl:value-of select="@id"/>
				</xsl:variable>
				<td align="center" valign="top">
				  <xsl:variable name="morphs" select="//Morpheme[@startclass=$sCLID and @endclass=$sCLID]"/>
				  <xsl:if test="$morphs">
					<!-- In the following xsl:choose element, we repeat the xsl:attribute in each part so XMLSpy will format it in a legible form;  if we don't, XMLSpy puts it all on the same line and it's a bear to read-->
					<xsl:choose>
					  <xsl:when test="@isfogbank='1'">
						<xsl:attribute name="style"><xsl:value-of select="$sBackColorAllBad"/></xsl:attribute>
					  </xsl:when>
					  <xsl:otherwise>
						<xsl:attribute name="style"><xsl:value-of select="$sBackColorAllGood"/></xsl:attribute>
					  </xsl:otherwise>
					</xsl:choose>
					<xsl:for-each select="$morphs">
					  <a>
						<xsl:attribute name="href">#<xsl:value-of select="@id"/></xsl:attribute>
						<xsl:value-of select="@id"/>
						<xsl:text>-</xsl:text>
					  </a>
					  <br/>
					</xsl:for-each>
				  </xsl:if>
				</td>
			  </xsl:for-each>
			</tr>
			<!-- prefixes in more than one column -->
			<xsl:for-each select="PrefixClasses/Class">
			  <xsl:sort select="position()" order="descending"/>
			  <xsl:variable name="sFirstCLID">
				<xsl:value-of select="@id"/>
			  </xsl:variable>
			  <xsl:variable name="sFirstIsFogBank">
				<xsl:value-of select="@isfogbank"/>
			  </xsl:variable>
			  <xsl:variable name="iFirstPosition" select="position()"/>
			  <xsl:for-each select="preceding-sibling::*">
				<xsl:sort select="position()" order="descending"/>
				<xsl:variable name="sThisCLID">
				  <xsl:value-of select="@id"/>
				</xsl:variable>
				<xsl:variable name="morphs" select="//Morpheme[@startclass=$sThisCLID and @endclass=$sFirstCLID]"/>
				<xsl:if test="$morphs">
				  <!-- Actually have some data to show -->
				  <xsl:variable name="sThisIsFogBank">
					<xsl:value-of select="@isfogbank"/>
				  </xsl:variable>
				  <xsl:variable name="iThisPosition" select="position()+1"/>
				  <xsl:variable name="iTemp" select="$iFirstPosition + $iThisPosition - 1"/>
				  <xsl:variable name="iLeftOverColumns" select="$iPrefixColumns - $iTemp"/>
				  <tr>
					<xsl:if test="$iFirstPosition!=1">
					  <!-- have to skip some columns at the beginning -->
					  <td>
						<xsl:attribute name="colspan"><xsl:value-of select="$iFirstPosition - 1"/></xsl:attribute>
						<xsl:value-of select="$sNonBreakingSpace"/>
					  </td>
					</xsl:if>
					<!-- output the spanning prefixes -->
					<td align="center" valign="top">
					  <xsl:attribute name="colspan"><xsl:value-of select="$iThisPosition"/></xsl:attribute>
					  <!-- In the following xsl:choose element, we repeat the xsl:attribute in each part so XMLSpy will format it in a legible form;  if we don't, XMLSpy puts it all on the same line and it's a bear to read-->
					  <xsl:choose>
						<xsl:when test="$sFirstIsFogBank='1' or $sThisIsFogBank='1'">
						  <xsl:attribute name="style"><xsl:value-of select="$sBackColorEndIsBad"/></xsl:attribute>
						</xsl:when>
						<xsl:when test="following-sibling::*[position() + $iThisPosition - 1 &gt;= $iFirstPosition and @isfogbank='1']">
						  <xsl:attribute name="style"><xsl:value-of select="$sBackColorSpansBad"/></xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
						  <xsl:attribute name="style"><xsl:value-of select="$sBackColorAllGood"/></xsl:attribute>
						</xsl:otherwise>
					  </xsl:choose>
					  <xsl:for-each select="$morphs">
						<a>
						  <xsl:attribute name="href">#<xsl:value-of select="@id"/></xsl:attribute>
						  <xsl:value-of select="@id"/>
						  <xsl:text>-</xsl:text>
						</a>
						<br/>
					  </xsl:for-each>
					</td>
					<!-- skip any columns to stem -->
					<xsl:if test="$iLeftOverColumns > 0">
					  <td>
						<xsl:attribute name="colspan"><xsl:value-of select="$iLeftOverColumns"/></xsl:attribute>
						<xsl:value-of select="$sNonBreakingSpace"/>
					  </td>
					</xsl:if>
				  </tr>
				</xsl:if>
			  </xsl:for-each>
			</xsl:for-each>
		  </table>
		</td>
		<!--
		 Then the Stem
		  -->
		<th valign="top">Stem</th>
		<!--
		 Then the suffixes
		  -->
		<td valign="top">
		  <table border="1">
			<!-- headers -->
			<tr>
			  <xsl:for-each select="SuffixClasses/Class">
				<xsl:choose>
				  <xsl:when test="@isfogbank = '1'">
					<th>
					  <xsl:attribute name="style"><xsl:value-of select="$sBackColorAllBad"/></xsl:attribute>
					  <xsl:value-of select="$sError"/>
					</th>
				  </xsl:when>
				  <xsl:otherwise>
					<th>
					  <xsl:value-of select="substring-after(@id, 'SP')"/>
					</th>
				  </xsl:otherwise>
				</xsl:choose>
			  </xsl:for-each>
			</tr>
			<tr>
			  <!-- suffixes in only one column -->
			  <xsl:for-each select="SuffixClasses/Class">
				<xsl:variable name="sCLID">
				  <xsl:value-of select="@id"/>
				</xsl:variable>
				<td align="center" valign="top">
				  <xsl:variable name="morphs" select="//Morpheme[@startclass=$sCLID and @endclass=$sCLID]"/>
				  <xsl:if test="$morphs">
					<!-- In the following xsl:choose element, we repeat the xsl:attribute in each part so XMLSpy will format it in a legible form;  if we don't, XMLSpy puts it all on the same line and it's a bear to read-->
					<xsl:choose>
					  <xsl:when test="@isfogbank='1'">
						<xsl:attribute name="style"><xsl:value-of select="$sBackColorAllBad"/></xsl:attribute>
					  </xsl:when>
					  <xsl:otherwise>
						<xsl:attribute name="style"><xsl:value-of select="$sBackColorAllGood"/></xsl:attribute>
					  </xsl:otherwise>
					</xsl:choose>
					<xsl:for-each select="$morphs">
					  <a>
						<xsl:attribute name="href">#<xsl:value-of select="@id"/></xsl:attribute>
						<xsl:text>-</xsl:text>
						<xsl:value-of select="@id"/>
					  </a>
					  <br/>
					</xsl:for-each>
				  </xsl:if>
				</td>
			  </xsl:for-each>
			</tr>
			<!-- suffixes in more than one column -->
			<xsl:for-each select="SuffixClasses/Class">
			  <xsl:variable name="sFirstCLID">
				<xsl:value-of select="@id"/>
			  </xsl:variable>
			  <xsl:variable name="sFirstIsFogBank">
				<xsl:value-of select="@isfogbank"/>
			  </xsl:variable>
			  <xsl:variable name="iFirstPosition" select="position()"/>
			  <xsl:for-each select="following-sibling::*">
				<xsl:variable name="sThisCLID">
				  <xsl:value-of select="@id"/>
				</xsl:variable>
				<xsl:variable name="morphs" select="//Morpheme[@startclass=$sFirstCLID and @endclass=$sThisCLID]"/>
				<xsl:if test="$morphs">
				  <!-- Actually have some data to show -->
				  <xsl:variable name="sThisIsFogBank">
					<xsl:value-of select="@isfogbank"/>
				  </xsl:variable>
				  <xsl:variable name="iThisPosition" select="position()+1"/>
				  <xsl:variable name="iTemp" select="$iFirstPosition + $iThisPosition - 1"/>
				  <xsl:variable name="iLeftOverColumns" select="$iSuffixColumns - $iTemp"/>
				  <tr>
					<!-- handle initial space -->
					<xsl:if test="$iFirstPosition!=1">
					  <td>
						<xsl:attribute name="colspan"><xsl:value-of select="$iFirstPosition - 1"/></xsl:attribute>
						<xsl:value-of select="$sNonBreakingSpace"/>
					  </td>
					</xsl:if>
					<!-- process the spanning suffixes -->
					<td align="center" valign="top">
					  <xsl:attribute name="colspan"><xsl:value-of select="$iThisPosition"/></xsl:attribute>
					  <!-- In the following xsl:choose element, we repeat the xsl:attribute in each part so XMLSpy will format it in a legible form;  if we don't, XMLSpy puts it all on the same line and it's a bear to read-->
					  <xsl:choose>
						<xsl:when test="$sFirstIsFogBank='1' or $sThisIsFogBank='1'">
						  <xsl:attribute name="style"><xsl:value-of select="$sBackColorEndIsBad"/></xsl:attribute>
						</xsl:when>
						<xsl:when test="preceding-sibling::*[position() + $iThisPosition - 1 &gt;= $iFirstPosition and @isfogbank='1']">
						  <xsl:attribute name="style"><xsl:value-of select="$sBackColorSpansBad"/></xsl:attribute>
						</xsl:when>
						<xsl:otherwise>
						  <xsl:attribute name="style"><xsl:value-of select="$sBackColorAllGood"/></xsl:attribute>
						</xsl:otherwise>
					  </xsl:choose>
					  <xsl:for-each select="$morphs">
						<a>
						  <xsl:attribute name="href">#<xsl:value-of select="@id"/></xsl:attribute>
						  <xsl:text>-</xsl:text>
						  <xsl:value-of select="@id"/>
						</a>
						<br/>
					  </xsl:for-each>
					</td>
					<!-- handle any final space -->
					<xsl:if test="$iLeftOverColumns>0">
					  <td>
						<xsl:attribute name="colspan"><xsl:value-of select="$iLeftOverColumns"/></xsl:attribute>
						<xsl:value-of select="$sNonBreakingSpace"/>
					  </td>
					</xsl:if>
				  </tr>
				</xsl:if>
			  </xsl:for-each>
			</xsl:for-each>
		  </table>
		</td>
	  </tr>
	</table>
  </xsl:template>
  <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Challenges template
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
  <xsl:template match="Challenges">
	<xsl:if test="count(Challenge) > 0">
	  <h1>2 Data Challenges</h1>
	  <div>
		<div style="margin-left: 0.25in; margin-right: 0.25in; margin-top: 0.25in; margin-bottom: 0.25in">
		  <p>The data presented the following challenge<xsl:if test="count(Challenge) > 1">s</xsl:if>:</p>
		  <ol type="circle">
			<xsl:for-each select="Challenge">
			  <li>
				<xsl:value-of select="./@message"/>.
			</li>
			</xsl:for-each>
		  </ol>
		  <p>There are three categories of affixes:</p>
		  <ul>
			<li>
			  <div>
				<xsl:attribute name="style"><xsl:value-of select="$sBackColorAllBad"/></xsl:attribute>
				<p>Red -- At least two of the affixes in this box occur in an inconsistent
order.  That is, in one word form, one precedes the other, while in another
word form, the same two affixes occur in reverse order.  This could be
caused by an of the following:</p>
				<ol>
				  <li>There is homophony between what are actually two distinct morphemes.</li>
				  <li>There is a derivational affix which may appear in more than one order position.</li>
				  <li>Category changing derivational affixes may apply to an inflected form and then the resulting new stem may be inflected.  (For example, an inflected verb may become nominalized and then it in turn is inflected with nominal inflection).</li>
				</ol>
				<p>If one of these is the case for you, try the following:</p>
				<ol>
				  <li>Make a distinction between the two homomorphs.</li>
				  <li>Remove the derivational affix from your data.</li>
				  <li>Do not use such data for determining affix positioning.</li>
				</ol>
			  </div>
			  <p>&#x20;</p>
			</li>
			<li>
			  <div>
				<xsl:attribute name="style"><xsl:value-of select="$sBackColorEndIsBad"/></xsl:attribute>
				<p>Blue -- Because of the inconsistent ordering among the affixes in the ???
group, it is not possible to unambiguously assign any affixes to a single
position class. Once the inconsistency is sorted out, some of these affixes
will end up being in a single position class while others may be in
spanning classes.</p>
			  </div>
			</li>
			<li>
			  <div>
				<xsl:attribute name="style"><xsl:value-of select="$sBackColorSpansBad"/></xsl:attribute>
				<p>Green -- These affixes are in spanning classes which are not affected by
the inconsistency in the data.</p>
			  </div>
			</li>
		  </ul>
		</div>
	  </div>
	</xsl:if>
  </xsl:template>
  <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
WordRecords  template
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
  <xsl:template match="WordRecords">
	<h2>
	  <xsl:value-of select="$sDataSectionNumber"/>.1 Words</h2>
	<p>The results are based on the following word<xsl:if test="count(WordRecord)>1">s</xsl:if>:</p>
	<p>
	  <xsl:attribute name="style"><xsl:value-of select="$sIndent"/></xsl:attribute>
	  <xsl:for-each select="WordRecord">
		<xsl:call-template name="OutputWordform"/>
		<br/>
	  </xsl:for-each>
	</p>
  </xsl:template>
  <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
Morphemes  template
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
  <xsl:template match="Morphemes">
	<h2>
	  <xsl:value-of select="$sDataSectionNumber"/>.2 Morphemes</h2>
	<p>The results are based on the following morpheme<xsl:if test="count(Morpheme)>1">s</xsl:if>:</p>
	<table border="1">
	  <xsl:attribute name="style"><xsl:value-of select="$sIndent"/></xsl:attribute>
	  <tr>
		<th>Identity</th>
		<th>Affix type</th>
		<th>Wordforms</th>
	  </tr>
	  <xsl:for-each select="Morpheme">
	  <xsl:sort select="@id"/>
		<xsl:variable name="sMorphID">
		  <xsl:value-of select="@id"/>
		</xsl:variable>
		<xsl:variable name="morphemeforecolor">
				<xsl:choose>
				  <xsl:when test="@startclass= 'SP0' and @endclass= 'SP0'">Red</xsl:when>
				  <xsl:when test="@startclass= 'SP0'">Purple</xsl:when>
				  <xsl:when test="@endclass= 'SP0'">Orange</xsl:when>
				  <xsl:otherwise>Black</xsl:otherwise>
				</xsl:choose>
		</xsl:variable>
		<tr>
		  <td valign="top">
			<xsl:attribute name="id"><xsl:value-of select="@id"/></xsl:attribute>
			<xsl:attribute name="style">color:<xsl:value-of select="$morphemeforecolor"/>; font-weight: bold</xsl:attribute>
			<xsl:value-of select="@id"/>
		  </td>
		  <td valign="top">
			<xsl:value-of select="@type"/>
		  </td>
		  <td>
			<table border="0" cellpadding="0" cellspacing="0">
			  <xsl:for-each select="//WordRecord[Prefixes/Affix/@id=$sMorphID or Suffixes/Affix/@id=$sMorphID or Stem/@id=$sMorphID]">
				<tr>
				  <xsl:variable name="sMID">
					<xsl:value-of select="$sMorphID"/>
				  </xsl:variable>
				  <!-- get all morphemes -->
				  <xsl:variable name="nsMorphs" select="Prefixes/Affix | Stem | Suffixes/Affix"/>
				  <!-- find position of one to highlight -->
				  <xsl:variable name="iPos">
					<xsl:for-each select="$nsMorphs">
					  <xsl:if test="@id = $sMorphID">
						<xsl:value-of select="position()"/>
					  </xsl:if>
					</xsl:for-each>
				  </xsl:variable>
				  <!-- output all preceding ones in one cell -->
				  <td align="right">
					<xsl:for-each select="$nsMorphs">
					  <xsl:if test="position() &lt; $iPos">
						<xsl:call-template name="OutputAMorph"/>
					  </xsl:if>
					</xsl:for-each>
				  </td>
				  <!-- output the highlighted one-->
				  <td>
					<xsl:attribute name="style"><xsl:value-of select="$sBackColorAffix"/></xsl:attribute>
					<xsl:for-each select="$nsMorphs">
					  <xsl:if test="position() = $iPos">
						<xsl:call-template name="OutputAMorph"/>
					  </xsl:if>
					</xsl:for-each>
				  </td>
				  <!-- output all following ones in one cell -->
				  <td align="left">
					<xsl:for-each select="$nsMorphs">
					  <xsl:if test="position() &gt; $iPos">
						<xsl:call-template name="OutputAMorph"/>
					  </xsl:if>
					</xsl:for-each>
				  </td>
				</tr>
			  </xsl:for-each>
			</table>
		  </td>
		</tr>
	  </xsl:for-each>
	</table>
  </xsl:template>
  <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
OutputAffix
	Output an affix
		Parameters: none
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
  <xsl:template name="OutputAffix">
	<!-- this is simple now, but we'll probably change it later -->
	<xsl:value-of select="@id"/>
  </xsl:template>
  <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
OutputAMorph
	Output a morpheme
		Parameters: none
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
  <xsl:template name="OutputAMorph">
	<xsl:choose>
	  <xsl:when test="name()='Stem'">
		<xsl:call-template name="OutputStem"/>
	  </xsl:when>
	  <xsl:otherwise>
		<xsl:if test="name(..)='Suffixes'">
		  <xsl:text>-</xsl:text>
		</xsl:if>
		<xsl:call-template name="OutputAffix"/>
		<xsl:if test="name(..)='Prefixes'">
		  <xsl:text>-</xsl:text>
		</xsl:if>
	  </xsl:otherwise>
	</xsl:choose>
  </xsl:template>
  <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
OutputStem
	Output a stem
		Parameters: none
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
  <xsl:template name="OutputStem">
	<!-- this is simple now, but we'll probably change it later -->
	&lt;<xsl:value-of select="@id"/>&gt;
  </xsl:template>
  <!--
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
OutputWordform
	Output a wordform
		Parameters: sMID - morpheme id of highlighted morph
- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
-->
  <xsl:template name="OutputWordform">
	<xsl:param name="sMID"/>
	<xsl:for-each select="Prefixes/Affix">
	<xsl:call-template name="OutputAMorph"/>
	</xsl:for-each>
	<xsl:for-each select="Stem">
	<xsl:call-template name="OutputAMorph"/>
	</xsl:for-each>
	<xsl:for-each select="Suffixes/Affix">
	<xsl:call-template name="OutputAMorph"/>
	</xsl:for-each>
  </xsl:template>
</xsl:stylesheet>
<!--
================================================================
Revision History
- - - - - - - - - - - - - - - - - - -
14-Oct-2002   Andy Black       Redo wordforms in morpheme section so that the highlighted morpheme remains in the same postion for all wordforms for a given morpheme
03-Oct-2002   Andy Black       Rework Challenges report, error info in chart and add wordforms to morpheme display
01-Oct-2002   Andy Black        Remove ??? in first row of a failed affix report.  Add more info to Challenges report.
06-May-2002  Andy Black        Create prefix and suffix subtables so that spanning suffixes appear closer to top of
													the chart
												  Add hyperlinks from chart to morphemes.
												  Add date
03-May-2002  Andy Black        Fleshed out version 1.
01-May-2002  Randy Regnier  Began working on Initial Draft
================================================================
 -->
