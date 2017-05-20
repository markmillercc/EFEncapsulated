
#### Encapsulated Collections with Entity Framework

To encapsulate the behavior of an entity means to control access to its parts. Often in EF applications, collection properties that should otherwise be private, are made public to allow for EF mapping. While sometimes acceptable, this is generally the wrong approach. If the model design dictates that a property should be private – it should be private. To alter the attributes or behavior of an entity in order to appease the persistence layer is a violation of the domain's integrity.

Entity Framework works wonders with anemic models and the use of public POCOs – which map seamlessly to your DB and take away all the pain of persistence. This falls short, though, in any moderately complex domain. To promote encapsulation and explicit behavior in the model, we need private members.

In order to accomplish this, EF needs to be provided access.

The <a href="https://github.com/markmillercc/EFEncapsulated/blob/master/EFEncapsulated/Extensions/EntityTypeConfigurationExtensions.cs" target="_blank">extention methods</a> in this project do just that, in a simple and straight-forward way.

Check it out in use in the default <a href="https://github.com/markmillercc/EFEncapsulated/blob/master/EFEncapsulated/DAL/Model1Context.cs" target="_blank">db context</a>.
