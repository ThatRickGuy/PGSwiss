﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.18444
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'This class was auto-generated by the StronglyTypedResourceBuilder
    'class via a tool like ResGen or Visual Studio.
    'To add or remove a member, edit your .ResX file then rerun ResGen
    'with the /str option, or rebuild your VS project.
    '''<summary>
    '''  A strongly-typed resource class, for looking up localized strings, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("PGSwiss.Resources", GetType(Resources).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Overrides the current thread's CurrentUICulture property for all
        '''  resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &lt;div id=&quot;vr&quot;&gt;
        '''		&lt;/div&gt;
        '''	&lt;/div&gt;
        '''&lt;/body&gt;.
        '''</summary>
        Friend ReadOnly Property Footer() As String
            Get
                Return ResourceManager.GetString("Footer", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &lt;div id=&quot;header&quot;&gt;
        '''	&lt;h1&gt;{0}&lt;/h1&gt;
        '''	&lt;h2 style=&quot;margin-top:-20px;&quot;&gt;Round {1} Pairings&lt;/h2&gt;
        '''	&lt;h2 style=&quot;margin-top:-20px;&quot;&gt;Scenario {2} &lt;/h2&gt;&lt;/div&gt;.
        '''</summary>
        Friend ReadOnly Property Header() As String
            Get
                Return ResourceManager.GetString("Header", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &lt;!DOCTYPE HTML PUBLIC &quot;-//W3C//DTD HTML 4.01//EN&quot; &quot;http://www.w3.org/TR/html4/strict.dtd&quot;&gt;
        '''&lt;html lang=&quot;en&quot;&gt;
        '''&lt;head&gt;
        '''	&lt;meta http-equiv=&quot;Content-Type&quot; content=&quot;text/html; charset=utf-8&quot;&gt;
        '''	&lt;title&gt;Simple 2 column CSS layout, step 3 | 456 Berea Street&lt;/title&gt;
        '''	&lt;meta name=&quot;description&quot; content=&quot;How to create a simple two column CSS layout with full width header and footer.&quot;&gt;
        '''	&lt;meta name=&quot;copyright&quot; content=&quot;Copyright (c) 2004 Roger Johansson&quot;&gt;
        '''	&lt;meta name=&quot;author&quot; content=&quot;Roger Johansson&quot;&gt;
        '''	&lt;style type=&quot;t [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property HTMLHeader() As String
            Get
                Return ResourceManager.GetString("HTMLHeader", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &lt;div id=&quot;Item&quot;&gt;
        '''	&lt;span id=&quot;Player&quot;&gt;{0}&lt;/span&gt;&lt;span id=&quot;Player_Handle&quot;&gt; ({1})&lt;br&gt;
        '''	&lt;span id=&quot;Table&quot;&gt;Table {2}&lt;/span&gt;
        '''	&lt;span id=&quot;Opponent&quot;&gt;vs {3}&lt;/span&gt;&lt;span id=&quot;Player_Handle&quot;&gt; ({4})&lt;br&gt;
        '''&lt;/div&gt;.
        '''</summary>
        Friend ReadOnly Property Item() As String
            Get
                Return ResourceManager.GetString("Item", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &lt;div id=&quot;LeftColumn&quot;&gt;
        '''	{0}
        '''&lt;/div&gt;.
        '''</summary>
        Friend ReadOnly Property LeftColumn() As String
            Get
                Return ResourceManager.GetString("LeftColumn", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Drawing.Bitmap.
        '''</summary>
        Friend ReadOnly Property PGSwiss() As System.Drawing.Bitmap
            Get
                Dim obj As Object = ResourceManager.GetObject("PGSwiss", resourceCulture)
                Return CType(obj,System.Drawing.Bitmap)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &lt;div id=&quot;RightColumn&quot;&gt;
        '''	{0}
        '''&lt;/div&gt;.
        '''</summary>
        Friend ReadOnly Property RightColumn() As String
            Get
                Return ResourceManager.GetString("RightColumn", resourceCulture)
            End Get
        End Property
    End Module
End Namespace
