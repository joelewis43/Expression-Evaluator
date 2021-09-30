# Expression Evaluator

## Problem Statement
Write a problem to evaluate arithmetic expressions. Input will be test expression strings.

## Considerations
- Demonstrate the ability to parse/evaluate arithmetic expressions
- Support **addition** and **multiplication** and be easily extensible to add other operations later
- Be designed and implemented in an OO manner
- Not use the "Shunting Yard" algorithm
- Contain a testing framework to validate the solution is functioning as desired
- How can the test approach be incorporated into a CI system
- Consider all error situations in validation and testing
- Documentation: comments when things are not obvious

## How to run the application
1. Clone the code to your local machine
2. Open a command prompt or bash shell and navigate to Expression-Evaluator/Solution
3. Open either Debug/ or Release/ (debug contains more console logging)
4. Run ExpressionEvaluator.exe to see the input format
5. Run ExpressionEvaluator.exe with an input to view the result

## How to run the test framework
1. Clone the code to your local machine
2. Open a command prompt or bash shell and navigate to Expression-Evaluator/Solution
3. Open either Debug/ or Release/ (debug contains more console logging)
4. Run TestFramework.exe to view the result of each test case and see how many total test cases failed

## How to add a test case
1. Navigate to the directory the TestFramework.exe is in (Expression-Evaluator/Solution/(Debug|Release))
2. Open the file TestInputs.txt
3. At the bottom of the file add the expression you want to test on a new line
4. Add another line and put the expected result on it
5. If the result is expected to fail or is invalid, put false as the expected result

e.g.  
...  
(5+ 2)*(2 *- 1)  
false  
(5 + 2) * 8  
56  
((3 * 2) + 18) * (((2 + 1) * 6 + 4) + 20)  
1008  
<New-Expression>  
<Expression-Result>  
