<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="text" indent="no" encoding="utf-8" />
  <xsl:template match="DocumentElement">
    <xsl:apply-templates select="Document" />
  </xsl:template>

  <xsl:template match="Document">
    <xsl:for-each select="Enrollee">
      <xsl:text>"</xsl:text><xsl:value-of select="EventName" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="EventStart" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="EventEnd" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="Location" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="Category" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="ReferenceNumber" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="Company" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="JobTitle" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="FullName" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="FirstName" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="LastName" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="Email" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="Phone" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="Street" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="PostalCode" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="City" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="Region" /><xsl:text>";</xsl:text>
      <xsl:text>"</xsl:text><xsl:value-of select="Country" /><xsl:text>";</xsl:text>
      <xsl:text>&#10;</xsl:text>
    </xsl:for-each>
  </xsl:template>

</xsl:stylesheet>