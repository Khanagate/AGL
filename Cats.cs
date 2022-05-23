using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

/* Programming challenge
A json web service has been set up at the url: http://agl-developer-test.azurewebsites.net/people.json

You need to write some code to consume the json and output a list of all the cats in alphabetical 
order under a heading of the gender of their owner.

You can write it in any language you like. You can use any libraries/frameworks/SDKs you choose.

Example:
Male
Angel
Molly
Tigger
Female
Gizmo
Jasper
Notes
Submissions will only be accepted via github or bitbucket
Use industry best practices
Use the code to showcase your skill.
*/

/* INPUT from people.json on website and local file
[{"name":"Bob","gender":"Male","age":23,"pets":[{"name":"Garfield","type":"Cat"},{"name":"Fido","type":"Dog"}]},
{"name":"Jennifer","gender":"Female","age":18,"pets":[{"name":"Garfield","type":"Cat"}]},
{"name":"Steve","gender":"Male","age":45,"pets":null},
{"name":"Fred","gender":"Male","age":40,"pets":[{"name":"Tom","type":"Cat"},{"name":"Max","type":"Cat"},{"name":"Sam","type":"Dog"},{"name":"Jim","type":"Cat"}]},
{"name":"Samantha","gender":"Female","age":40,"pets":[{"name":"Tabby","type":"Cat"}]},
{"name":"Alice","gender":"Female","age":64,"pets":[{"name":"Simba","type":"Cat"},{"name":"Nemo","type":"Fish"}]}]
*/ 

// TODO
// Demonstrate some Inheritance knowledge, pets and multiple types of pets exist, owners have multiple pets
//      Split Pet as a class into children {Didn't actually need this}
//      Only implement Cat as a subset of Pet {Didn't actually need this}
// Parse JSON Input
//      Find way to interpret JSON stream to C# - DONE
//      Then parse C# strings, find way to split them - DONE
//      
// Display cats in alphabetical order under owner and gender - DONE

// RESEARCH
// https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-6-0
// Safe way to check JSON types {Might do this if I have time}
// https://makolyte.com/csharp-deserialize-json-to-a-derived-type/
// Visualise what C# class and properties the JSON objects will need
// https://json2csharp.com/


/* public class PersonConverter : JsonConverter<Owner>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(Owner).IsAssignableFrom(typeToConvert);
    }

    public override Owner Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (var jsonDoc = JsonDocument.ParseValue(ref reader))
        {
            //if the property isn't there, let it blow up
            switch (jsonDoc.RootElement.GetProperty("Type").GetString())
            {
                case nameof(Owner):
                    return jsonDoc.RootElement.Deserialize<Owner>(options);
                //warning: If you're not using the JsonConverter attribute approach,
                //make a copy of options without this converter
                default:
                    throw new JsonException("'Type' doesn't match a known derived type");
            }

        }
    }

    public override void Write(Utf8JsonWriter writer, Owner owner, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (object)owner, options);
        //warning: If you're not using the JsonConverter attribute approach,
        //make a copy of options without this converter
    }
}*/

public class Owner
{
    public string name { get; set; }
    public string gender { get; set; }
    public int age { get; set; }
    public List<Pet> pets { get; set; }
    
    /* public Owner()
    {

    } */

    public Owner(string name, string gender, int age, List<Pet> pets)
    {
        this.name = name;
        this.gender = gender;
        this.age = age;
        this.pets = pets;
    }

    // Testing purposes, extendable to other types of pets
    public void showPets()
    {
        if (pets !=null)
        {
            Console.WriteLine(name + " owns " + pets.Count + " pets");
            foreach(Pet pet in pets)
            {
                pet.display();
            }
        }
    } 

    public void showCats()
    {
        if (pets != null)
        {
            foreach(Pet pet in pets)
            {
                if (pet.type == "Cat")
                {
                    Console.WriteLine(pet.name);
                }
            }
        }
    } 

    public void sortPetNames()
    {
        if (pets !=null)
        {
            pets.Sort((x, y) => string.Compare(x.name, y.name));
        }
    }
}

public class Pet
{
    public string name { get; set; }
    public string type { get; set; }
    
    public Pet(string name, string type)
    {
        this.name = name;
        this.type = type;
    }

    public void display()
    {
        Console.WriteLine(name + " is a " + type);
    }
}

// How to download JSON from a URL for C#
// https://stackoverflow.com/questions/37672423/deserialize-json-array-with-json-net-from-url
// Deserialise JSON into C# just using standard System.Text
// https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-6-0

public class Program 
{
    // https://www.quora.com/How-do-I-compile-C-in-Visual-Studio-Code
    // Testing purposes, uses copy of Json file in project files
    // Was considering NewtonJsoft but I thought System.Text.Json seemed adequate
    // https://inspiration.nlogic.ca/en/a-comparison-of-newtonsoft.json-and-system.text.json
    static void parse(ref List<Owner> owners)
    {
        
        string fileName = "people.json";
        string jsonString = File.ReadAllText(fileName);
        var deserialize = JsonSerializer.Deserialize<List<Owner>>(jsonString)!;
        if (deserialize != null)
        {
            owners = deserialize;
        }
    }

    // Uses obsolete Webclient()
    // Httpclient() forced you to run functions as Tasks asynchronously and as a void
    // Couldn't get Httpclient() to work so I swapped back to the much simpler Webclient()
    // Assuming connection to http://agl-developer-test.azurewebsites.net is stable
    // This functionality to read JSON directly from the web took the most development time
    // https://docs.microsoft.com/en-us/dotnet/api/system.net.webclient.openread?view=net-6.0
    static void parseWeb(ref List<Owner> owners)
    {

        var client = new WebClient();
        try
        {
            // send request   
            Stream jsonStream = client.OpenRead("http://agl-developer-test.azurewebsites.net/people.json");
            var deserialize = JsonSerializer.Deserialize<List<Owner>>(jsonStream)!;
            if (deserialize != null)
            {
                owners = deserialize;
            }
        }

        catch (WebException exception)
        {
            string responseText;

            var responseStream = exception.Response?.GetResponseStream();

                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseText = reader.ReadToEnd();
                        Console.WriteLine(responseText);
                    }
                }

        }
 
    }
    
    public static void Main()
    {
        List<Owner> owners = new List<Owner>();
        parseWeb(ref owners);
        foreach(Owner owner in owners)
        {
            owner.sortPetNames();
            if (owner.pets != null)
            {
                Console.WriteLine("=== " + owner.gender + " owner ===");
                owner.showCats();
            }
        }
    }
}

