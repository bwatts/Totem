All references to this project should be from disk, not through a project reference. There are also Project Dependencies on it, to ensure VS builds it first.

This allows the fluent test interface to hide the members inherited from System.Object as outlined here:

http://stackoverflow.com/questions/1464737/hiding-gethashcode-equals-tostring-from-fluent-interface-classes-intellisense-in/1473825#1473825

However, a project reference causes Visual Studio to ignore EditorBrowsable attributes. We deserve nice syntax too!

Totem.Testing should change slowly enough that it does not cause issues with tests in this solution.

This idea is also captured by the Totem.IFluent interface, but we don't want to require the disk reference for the core project.