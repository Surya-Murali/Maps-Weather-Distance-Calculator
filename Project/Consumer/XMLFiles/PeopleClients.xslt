<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />
	<xsl:template match="/">
		<xsl:for-each select="//Person[Role='Client']">
			<xsl:value-of select="@id"/>|<xsl:value-of select="Name/Last"/>|<xsl:value-of select="Name/@suffix"/>|<xsl:value-of select="Name/First"/>|<xsl:value-of select="WorkLocation/StreetAddress"/>|<xsl:value-of select="WorkLocation/City"/><xsl:text>&#xa;</xsl:text>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
