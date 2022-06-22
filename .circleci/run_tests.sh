#!/bin/bash

set -e 

if [ "$CI" == "true" ]; then
	echo "Using CI"
	UNITY_BIN="/opt/Unity/Editor/Unity"
else
	echo "Using local Unity"
	UNITY_BIN="/Applications/Unity/Hub/Editor/2020.3.21f1/Unity.app/Contents/MacOS/Unity"
fi

TEST_RESULTS_FILE="Build/Game Doctor/tests.xml"
mkdir -p "Build/Game Doctor/"

# GENERIC TESTS
echo "Executing Generic Tests..."

$UNITY_BIN -runTests -projectPath . -runSynchronously -nographics -batchmode -testPlatform "EditMode" -buildTarget "Android" -testResults "Build/Game Doctor/tests.xml" -nolog || true

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

$UNITY_BIN -runTests -projectPath . -runSynchronously -nographics -batchmode -testPlatform "EditMode" -buildTarget "iOS" -testResults "Build/Game Doctor/tests.xml" -nolog || true

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