using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJson;

public class GetDBData : MonoBehaviour {

    private GetDBData db;
    private WWW connection;
    private JSONObject jsonObect;
    private string output;

	// Use this for initialization
	void Start () {
        Debug.Log("Starting the GetDBData Start() Method");

        string url = "http://localhost:8080/?resultRequest=testTable";

        connection = GET(url);

        jsonObect = null;
        output = "";
    }

    public WWW GET(string url)
    {
        WWW www = new WWW(url);
        StartCoroutine(WaitForRequest(www));
        return www;
    }

    public WWW POST(string url, Dictionary<string,string> post)
    {
        WWWForm form = new WWWForm();
        foreach(KeyValuePair<String, String> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }

        WWW www = new WWW(url, form);

        StartCoroutine(WaitForRequest(www));
        return www;
    }

    private IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        //check for errors
        if(www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (connection.isDone && connection.error == null && jsonObect == null)
        {
            Debug.Log("Testing if this gets run after the GET: " + connection.text);
            jsonObect = new JSONObject(connection.text);

            Debug.Log("Decoding JSON Object:");
            accessData(jsonObect);

            Debug.Log(output);
        }
	}

    void accessData(JSONObject obj)
    {
        switch (obj.type)
        {
            case JSONObject.Type.OBJECT:
                output += "{";
                for (int i = 0; i < obj.list.Count; i++)
                {
                    string key = (string)obj.keys[i];
                    JSONObject j = (JSONObject)obj.list[i];
                    output += key + "->";
                    accessData(j);
                }
                output += "}";
                break;

            case JSONObject.Type.ARRAY:
                bool first = true;
                output += "[";
                foreach (JSONObject j in obj.list)
                {
                    if (!first)
                    {
                        output += ",";
                    }
                    accessData(j);
                    first = false;
                }
                output += "]";
                break;

            case JSONObject.Type.STRING:
                output += '"' + obj.str + '"';
                break;

            case JSONObject.Type.NUMBER:
                output += obj.n;
                break;

            case JSONObject.Type.BOOL:
                output += obj.b;
                break;

            case JSONObject.Type.NULL:
                output += "NULL";
                break;
        }
    }
}
