
pipeline
{
	agent
	{
		label "windows"
	}
	stages
	{
		stage ('Restore') {
			steps {
				bat "dotnet restore source/CsQuery.sln"
			}
		}

		stage('Test') {
			steps {
				bat "dotnet test source/CsQuery.Tests"
			}
		}

		stage('Pack') {
			steps {
				bat "dotnet pack source/HtmlParserSharp"
				bat "dotnet pack source/CsQuery"
			}
		}
	}
}
