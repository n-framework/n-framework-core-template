#!/usr/bin/env bash
# Shared initialization helpers for package scripts.
# Source this file: source "$(dirname "${BASH_SOURCE[0]}")/common.sh"
# The calling script must set REPO_ROOT before sourcing.

set -euo pipefail

ensure_tools() {
	local tool_manifest
	tool_manifest="$(git -C "${REPO_ROOT}" rev-parse --show-toplevel)/.config/dotnet-tools.json"
	echo "🔧 Restoring .NET tools..."
	dotnet tool restore --verbosity quiet --tool-manifest "${tool_manifest}"
}

init_environment() {
	ensure_tools
}
