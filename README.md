# EFEncapsulated

To encapsulate the behavior of an entity means to control access to its parts. Often in EF applications, collection properties that should otherwise be private, are made public to allow for EF mapping. While sometimes acceptable, this is generally the wrong approach. If the model design dictates that a property should be private – it should be private. To alter the attributes or behavior of an entity in order to appease the persistence layer is a violation of the domain's integrity.

Entity Framework works wonders with anemic models and the use of public POCOs – which map seamlessly to your DB and take away all the pain of persistence. This falls short, though, in any moderately complex domain. To promote encapsulation and explicit behavior in the model, we need private attributes.

Here’s a classic example, simplified:

<script src="https://gist.github.com/markmillercc/dd1b2821ef651b088faa19bf150d502e.js"></script>
<script src="https://gist.github.com/nisrulz/11c0d63428b108f10c83.js"></script>

Entity Framework has two problems with this.

First, it just won’t map fields. <code>_LineItems</code> must be changed to a property. Fortunately, C# auto properties make this largely a non-issue.

Second, even as a property, <code>_LineItems</code> is still private; inaccessible by design. We need to give EF access. One method, described <a href="http://ardalis.com/exposing-private-collection-properties-to-entity-framework" target="_blank">here</a> and <a href="http://owencraig.com/mapping-but-not-exposing-icollections-in-entity-framework/" target="_blank">here</a> for example, involves exposing an expression property on the domain entity itself. As clever and simple as this is, I'm not a big fan - it's not the responsibility of the domain entity to map itself to a particular persistence layer. 

Here’s my solution, a simple extension to <code>EntityTypeConfiguration</code>:

<script src="https://gist.github.com/markmillercc/eae60c27ac2975919984e4ed90ae0dee.js"></script>

In use:

<script src="https://gist.github.com/markmillercc/8e8024661804d53645231047126efa6d.js"></script>

The biggest downfall to this method is that we have to pass in a string - not a deal breaker, but kind of annoying. Jimmy Bogard has <a href="https://lostechies.com/jimmybogard/2014/05/09/missing-ef-feature-workarounds-encapsulated-collections/" target="_blank">another nice workaround</a>. It's a little more complicated, but also a little more robust.

For now, though, I'm fine with passing in a string - with decent naming conventions, this poses little risk. Besides, we're not touching the domain objects themselves, so these mapping details can be easily refactored later.
