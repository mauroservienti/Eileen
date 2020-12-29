# Eileen

A trivially simple library management tool for my dad

## How to run tests locally

Tests require a Microsoft SQL Server instance. By default tests assume a LocalDB instance is available at `(localdb)\Eileen`, and will use the following connection string: `Data Source=(localdb)\Eileen;Integrated Security=True`.

To provide a custom connection string define an environment variable named `EILEEN_TESTS_CONNECTION_STRING` to store the tests connection string.

The connection string should not contain the `Instance Catalog` parameter; if it's defined its value will be replaced with a randomly generated instance catalog name for each test execution.
