{ nixpkgs ? import <nixpkgs> { } }:
let pkgs = {
  atomi = (
    with import (fetchTarball "https://github.com/kirinnee/test-nix-repo/archive/refs/tags/v8.2.1.tar.gz");
    {
      inherit pls dotnet-ef;
    }
  );
  "Unstable 25th Janurary 2021" = (
    with import (fetchTarball "https://github.com/NixOS/nixpkgs/archive/2d77d1ce9018.tar.gz") { };
    {
      inherit
        git
        envsubst
        hadolint
        ansible-lint
        docker-compose
        ansible
        docker
        dotnet-sdk
        pre-commit
        shfmt
        shellcheck
        nixpkgs-fmt
        bash
        coreutils
        jq
        gnugrep;
      prettier = nodePackages.prettier;
    }
  );
}; in
with pkgs;
pkgs.atomi // pkgs."Unstable 25th Janurary 2021"
