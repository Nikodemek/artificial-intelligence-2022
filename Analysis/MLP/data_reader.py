import json
import os

class TestResult(object):
    def __init__(
        self,
        inputData,
        expectedResults,
        actualResults,
        guessingAccuracy,
        guessingClassAccuracy,
        entireError,
        individualErrors,
        outputNeuronsWeights,
        hiddenNeuronsValues,
        hiddenNeuronsWeights):
        self.input_data = inputData
        self.expected_results = expectedResults,
        self.actual_results = actualResults,
        self.guessing_accuracy = guessingAccuracy,
        self.guessing_class_accuracy = guessingClassAccuracy,
        self.entire_error = entireError,
        self.individual_errors = individualErrors,
        self.output_neurons_weights = outputNeuronsWeights,
        self.hidden_neurons_values = hiddenNeuronsValues,
        self.hidden_neurons_weights = hiddenNeuronsWeights
        pass


dirName = os.path.dirname(__file__)

def read_results_file():
    with open(os.path.join(dirName, "result.json")) as dataFile:
        return dataFile.read()

def as_test_result(dct):
    return TestResult(
        dct['InputData'],
        dct['ExpectedResults'],
        dct['ActualResults'],
        dct['GuessingAccuracy'],
        dct['GuessingClassAccuracy'],
        dct['EntireError'],
        dct['IndividualErrors'],
        dct['OutputNeuronsWeights'],
        dct['HiddenNeuronsValues'],
        dct['HiddenNeuronsWeights'])

def main():
    rawJson = read_results_file()
    results : TestResult = as_test_result(json.loads(rawJson))
    
    for i in results.hidden_neurons_weights:
        for j in i:
            for k in j:
                print(k)


if (__name__=='__main__'):
    main()