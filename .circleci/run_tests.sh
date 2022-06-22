#!/bin/bash

curl -H "Authorization: token $GITHUB_TOKEN" "https://raw.githubusercontent.com/homagames/hg-cicd-common/master/mobile/licenses/activate_unity_license.sh" | bash

set -e 

TEST_RESULTS_FILE="Build/Game Doctor/tests.xml"
mkdir -p "Build/Game Doctor/"

# GENERIC TESTS
echo "Executing Generic Tests..."

unity-editor -runTests -projectPath . -runSynchronously -nographics -batchmode -testPlatform "EditMode" -buildTarget "Android" -testResults "Build/Game Doctor/tests.xml" -nolog || true

TESTS_COUNT=$(grep 'HomaGames.GameDoctor.Tests.dll' "$TEST_RESULTS_FILE" | grep -o 'testcasecount=\"[0-9][0-9]*\"' | grep -o '[0-9][0-9]*')
PASSED_TESTS=$(grep 'HomaGames.GameDoctor.Tests.dll' "$TEST_RESULTS_FILE" | grep -o 'passed=\"[0-9][0-9]*\"' | grep -o '[0-9][0-9]*')
FAILED_TESTS=$(grep 'HomaGames.GameDoctor.Tests.dll' "$TEST_RESULTS_FILE" | grep -o 'failed=\"[0-9][0-9]*\"' | grep -o '[0-9][0-9]*')
if [ $FAILED_TESTS == "0" ]; then
  echo "Generic tests: $PASSED_TESTS/$TESTS_COUNT tests passed"
else
  echo "Generic tests: $FAILED_TESTS/$TESTS_COUNT tests failing:"
  cat "$TEST_RESULTS_FILE"
  exit 1
fi

# iOS TESTS
echo "Executing iOS Tests..."

unity-editor -runTests -projectPath . -runSynchronously -nographics -batchmode -testPlatform "EditMode" -buildTarget "iOS" -testResults "Build/Game Doctor/tests.xml" -nolog || true

TESTS_COUNT=$(grep 'HomaGames.GameDoctor.Tests.dll' "$TEST_RESULTS_FILE" | grep -o 'testcasecount=\"[0-9][0-9]*\"' | grep -o '[0-9][0-9]*')
PASSED_TESTS=$(grep 'HomaGames.GameDoctor.Tests.dll' "$TEST_RESULTS_FILE" | grep -o 'passed=\"[0-9][0-9]*\"' | grep -o '[0-9][0-9]*')
FAILED_TESTS=$(grep 'HomaGames.GameDoctor.Tests.dll' "$TEST_RESULTS_FILE" | grep -o 'failed=\"[0-9][0-9]*\"' | grep -o '[0-9][0-9]*')
if [ $FAILED_TESTS == "0" ]; then
  echo "iOS tests: $PASSED_TESTS/$TESTS_COUNT tests passed"
else
  echo "iOS tests: $FAILED_TESTS/$TESTS_COUNT tests failing:"
  cat "$TEST_RESULTS_FILE"
  exit 1
fi

curl -H "Authorization: token $GITHUB_TOKEN" "https://raw.githubusercontent.com/homagames/hg-cicd-common/master/mobile/licenses/return_unity_license.sh" | bash