#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
source "${SCRIPT_DIR}/common.sh"

init_environment

echo "🔨 Building NFramework.Core.Template..."
dotnet build "${REPO_ROOT}/NFramework.Core.Template.slnx"

echo "✅ Build complete!"
