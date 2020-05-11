# Agrix

Creates infrastructure from (YAML) configuration to help you maintain
infrastructure as code. It takes a YAML file, and then makes the necessary API
calls to build infrastructure matching that configuration.

Agrix is designed to be simple. It takes a YAML file and spins up
infrastructure. That's it. It doesn't have additional complexities like loops,
variables or conditionals. If any of those features are needed, it is
recommended to either use a more robust tool like [terraform][1] or apply
pre-processing before sending YAML to agrix (e.g. use [envsubst][2] to
substitute variables inside the YAML config.)

Agrix is intended to be used in a hands-free manner without human intervention
or oversight. It can be run from the command line or inside a container.

[1]: https://github.com/hashicorp/terraform
[2]: https://github.com/a8m/envsubst

## Usage

Agrix takes a single YAML file as input:

    agrix [file]

Alternatively, you can also pipe the file into agrix:

    envsubst [file] | agrix
    agrix < [file]

### Container Usage

A base container image is provided: `okinta/agrix`. You can override the
`/etc/agrix.yaml` file and agrix will load it upon launch.

    FROM okinta/agrix
    COPY agrix.yaml /etc

By default environment variables will be substituted inside `agrix.yaml` via
envsubst.

## Supported Platforms

Currently only [Vultr][1] is supported as a platform. PRs are welcome to add
support for other platforms.

[1]: https://www.vultr.com/

## Development

Testing and building can be conducted via Visual Studio or another similar
tool. Testing can also be conducted via the `dotnet` command line tool or via
containers.

### Running Tests via dotnet

Building and testing can be conducted via the [dotnet][1] tool:

    dotnet build agrix.csproj
    dotnet test agrix.csproj

[1]: https://dotnet.microsoft.com/download/dotnet/5.0

### Running Tests via Container

Tests can be run via [Podman][1] or other similar container tools:

    podman build -t agrix-tests -f tests.Containerfile .
    podman run agrix-tests

For example, to run tests using [Docker][2]:

    docker build -t agrix-tests -f tests.Containerfile .
    docker run agrix-tests

[1]: https://podman.io/
[2]: https://docs.docker.com/
