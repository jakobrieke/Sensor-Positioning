using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cairo;

namespace charlie
{
  public interface ISimulation
  {
    string GetTitle();
    string GetDescr();
    string GetMeta();
    string GetConfig();
    void Init(Dictionary<string, string> config);
    void End();
    void Update(long deltaTime);
    byte[] Render(int width, int height);
    string Log();
  }
  
  public abstract class AbstractSimulation : ISimulation
  {
    private ImageSurface _surface;
    private Context _ctx;
    public abstract string GetTitle();
    public abstract string GetDescr();
    public abstract string GetConfig();

    public virtual string GetMeta()
    {
      return null;
    }

    public static string GetAnyOf(Dictionary<string, string> model,
      string key, List<string> possibleValues, string backup)
    {
      if (!model.TryGetValue(key, out var value)) return backup;
      return possibleValues.Contains(value) ? value : backup;
    }
    
    public static double GetDoubleInRange(Dictionary<string, string> config,
      string key, double min, double max, double backup)
    {
      var value = GetDouble(config, key, backup);
      return value > min && value < max ? value : backup;
    }
    
    public static double GetDouble(Dictionary<string, string> config,
      string key, double backup)
    {
      if (!config.ContainsKey(key)) return backup;
      return float.TryParse(config[key], out var x) ? x : backup;
    }
    
    public static int GetIntInRange(Dictionary<string, string> config,
      string key, int min, int max, int backup)
    {
      var value = GetInt(config, key, backup);
      return value > min && value < max ? value : backup;
    }

    public static int GetInt(Dictionary<string, string> config,
      string key, int backup)
    {
      if (!config.ContainsKey(key)) return backup;
      return int.TryParse(config[key], out var x) ? x : backup;
    }
    
    public static double[] ParseVector(string source)
    {
      var pos = source
        .Replace("]", "")
        .Replace("[", "")
        .Split(',');
      var result = new double[pos.Length];
        
      for (var i = 0; i < pos.Length; i++)
      {
        if (float.TryParse(pos[i], out var number)) result[i] = number;
        else return null;
      }

      return result;
    }

    public static double[] GetVector(
      IReadOnlyDictionary<string, string> config, string key)
    {
      return config.ContainsKey(key) ? ParseVector(config[key]) : null;
    }
    
    public static double[][] ParseMatrix(string source)
    {
      if (string.IsNullOrEmpty(source)) return null;
      
      source = source.Replace(" ", "");
      source = source.Substring(1, source.Length - 2);

      if (source == "") return new double[0][];
      
      var positions = Regex.Split(source, "],");
      var result = new double[positions.Length][];
      
      for (var i = 0; i < positions.Length; i++)
      {
        var pos = positions[i]
          .Replace("]", "")
          .Replace("[", "")
          .Split(',');
        result[i] = new double[pos.Length];
        
        for (var j = 0; j < pos.Length; j++)
        {
          if (float.TryParse(pos[j], out var number)) result[i][j] = number;
          else return null;
        }
      }
      return result;
    }
    
    public static double[][] GetMatrix(
      IReadOnlyDictionary<string, string> config, string key)
    {
      return config.ContainsKey(key) ? ParseMatrix(config[key]) : null;
    }
    
    public abstract void Init(Dictionary<string, string> config);
    
    public void End()
    {
      _surface.Dispose();
      _ctx.Dispose();
    }

    public abstract void Update(long deltaTime);
    public abstract void Render(Context cr, int width, int height);
    
    public virtual byte[] Render(int width, int height)
    {
      try
      {
        if (_surface != null)
        {
          _surface.Dispose();
          _ctx.Dispose();
        }
        _surface = new ImageSurface(Format.ARGB32, width, height);
        _ctx = new Context(_surface);
//        _ctx.Scale(2, 2);
        Render(_ctx, width, height);
        return _surface.Data;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        return null;
      }
    }

    public virtual string Log()
    {
      return null;
    }
  }
}