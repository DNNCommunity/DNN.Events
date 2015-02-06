Imports System
Namespace DNNtc

#Region " Attributes "
    ''' <summary>
    ''' This class is used to set information about the package
    ''' </summary>
    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=True)> _
    Public Class PackageProperties
        Inherits Attribute
        ' ReSharper disable UnusedParameter.Local
        ''' <summary>
        ''' Creates a attribute with the right properties to create a package. Use on View element
        ''' </summary>
        ''' <param name="name">The package name</param>
        ''' <param name="viewOrder">Ordinal number for sorting packages in the manifest</param>
        ''' <param name="friendlyName">The package friendly name</param>
        ''' <param name="description">The package description</param>
        ''' <param name="iconFile">The package iconfile name</param>
        ''' <param name="ownerName">Name of Owner</param>
        ''' <param name="ownerOrganization">Organization of owner</param>
        ''' <param name="ownerUrl">Url of owner</param>
        ''' <param name="ownerEmail">Email of owner</param>

        Public Sub New(ByVal name As String, ByVal viewOrder As Integer, ByVal friendlyName As String, ByVal description As String, ByVal iconFile As String, ByVal ownerName As String, ByVal ownerOrganization As String, ByVal ownerUrl As String, ByVal ownerEmail As String)
            'Intentially left empty
        End Sub

        ''' <summary>
        ''' Creates a attribute with the right properties to create a package. Use on other elements
        ''' </summary>
        ''' <param name="name">The package name</param>
        Public Sub New(ByVal name As String)
            ' ReSharper restore UnusedParameter.Local
            'Intentially left empty
        End Sub

    End Class

    ''' <summary>
    ''' This class is used to set information about the module
    ''' </summary>
    <AttributeUsage(AttributeTargets.Class, AllowMultiple:=True)> _
    Public Class ModuleProperties
        Inherits Attribute
        ' ReSharper disable UnusedParameter.Local
        ''' <summary>
        ''' Creates a attribute with the right properties to create a module. Use on View element
        ''' </summary>
        ''' <param name="name">The module name.</param>
        ''' <param name="friendlyname">The module friendlyname.</param>
        ''' <param name="defaultCacheTime">the module default cachetime.</param>

        Public Sub New(ByVal name As String, ByVal friendlyname As String, ByVal defaultCacheTime As Integer)
            'Intentially left empty
        End Sub

        ''' <summary>
        ''' Creates a attribute with the right properties to create a module. Use on other elements
        ''' </summary>
        ''' <param name="name">The module name.</param>
        Public Sub New(ByVal name As String)
            ' ReSharper restore UnusedParameter.Local
            'Intentially left empty
        End Sub

    End Class

    ''' <summary>
    ''' This class is used to indicate which UserControls should be in the install package
    ''' </summary>
    Public Class ModuleControlProperties
        Inherits Attribute
        ' ReSharper disable UnusedParameter.Local
        ''' <summary>
        ''' Initializes a new instance of the <see cref="ModuleControlProperties" /> class.
        ''' </summary>
        ''' <param name="key">The key.</param>
        ''' <param name="title">The title.</param>
        ''' <param name="userControlType">Type of the user control.</param>
        ''' <param name="helpUrl">The help URL.</param>
        ''' <param name="supportsPartialRendering">if set to <c>true</c> [supports partial rendering].</param>
        ''' <param name="supportsPopUps">if set to <c>true</c> [supports pop ups].</param>

        Public Sub New(ByVal key As String, ByVal title As String, ByVal userControlType As ControlType, ByVal helpUrl As String, ByVal supportsPartialRendering As Boolean, ByVal supportsPopUps As Boolean)
            ' ReSharper restore UnusedParameter.Local
            'Intentially left empty
        End Sub

    End Class

    ''' <summary>
    ''' Module permission attribute
    ''' </summary>
    <AttributeUsage(AttributeTargets.[Class], AllowMultiple:=True)> _
    Public Class ModulePermission
        Inherits Attribute
        ' ReSharper disable UnusedParameter.Local
        ''' <summary>
        ''' Empty Constructor for Intellisense
        ''' </summary>
        ''' <param name="code"></param>
        ''' <param name="key"></param>
        ''' <param name="name"></param>
        Public Sub New(ByVal code As String, ByVal key As String, ByVal name As String)
            ' ReSharper restore UnusedParameter.Local
            'Intentially left empty
        End Sub
    End Class

    Public Class BusinessControllerClass
        Inherits Attribute
        'Intentially left empty
    End Class

    Public Class UpgradeEventMessage
        Inherits Attribute
        ' ReSharper disable UnusedParameter.Local
        Public Sub New(ByVal versionList As String)
            ' ReSharper restore UnusedParameter.Local
            'Intentially left empty
        End Sub
    End Class

    <AttributeUsage(AttributeTargets.[Class], AllowMultiple:=True)> _
    Public Class ModuleDependencies
        Inherits Attribute
        ' ReSharper disable UnusedParameter.Local
        Public Sub New(ByVal type As ModuleDependency, ByVal value As String)
            ' ReSharper restore UnusedParameter.Local
            'Intentially left empty
        End Sub
    End Class

#End Region

#Region " Enums "

    Public Enum ControlType
        SkinObject
        Anonymous
        View
        Edit
        Admin
        Host
    End Enum

    Public Enum ModuleDependency
        CoreVersion
        Package
        Permission
        Type
    End Enum

#End Region

End Namespace