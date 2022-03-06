{ nixpkgs ? import <nixpkgs> { } }:
let pkgs = import ./packages.nix { inherit nixpkgs; }; in
with pkgs;
{
  system = [
    coreutils
    gnugrep
  ];

  dev = [
    dotnet-sdk
    dotnet-ef
    docker
    docker-compose
    pls
    git
  ];

  lint = [
    pre-commit
    ansible-lint
    hadolint
    nixpkgs-fmt
    prettier
    shfmt
    shellcheck
  ];

  deploy = [
    openssh
    ansible
    envsubst
  ];

}
