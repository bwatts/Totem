using System.Runtime.CompilerServices;

// The fixture base classes declare .CreateHost and .GetOrStartHost as internal,
// keeping those details out of the direct sight of test writers.
//
// Timeline integration fixtures also follow that pattern, but as they reside
// in a separate assembly, need explicit access to those methods.

[assembly: InternalsVisibleTo("Totem.Timeline.IntegrationTests")]