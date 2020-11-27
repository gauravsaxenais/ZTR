# EDD Modules

This repository defines the static configuration of EDD modules in proto form,
such that it can be consumed by the module itself for decoding as well as the
module compiler.

A template for the structure of a module's configuration can be found
[here](module.proto.example).


# Maintaining

## Adding a New Module

A module's config proto should be named `module.proto`, and placed into a folder
with a name identical to the source code repository for the module's
implementation, followed by a folder named after a UUID which must be generated
for each new module.

An example:

```
gpio
└── 339248087cbc49c0bf74f8915893573d
    └── module.proto
```

The module's proto should start from the example template above. New message
types or enums within these protos are not supported.


## Updating a Module

If a module's config has been updated in a forward compatible way (see breaking
changes below) the uuid need not be updated. If a breaking change must be made,
a new UUID should be generated under the module's named folder, with a new
`module.proto` within.

Breaking changes can include (but are not limited to):

-   Changing the numeric value of an event, filter, or action.
-   Changing the field ID of a field in the Config message.
-   Removing a field ID (unless reserved and commented as deprecated.)


# Releasing

Updates to this repository should be tagged and propagated into the appropriate
module compiler (ie. `goat-modules`.)
