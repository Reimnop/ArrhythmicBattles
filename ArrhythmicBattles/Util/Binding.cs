using System.ComponentModel;

namespace ArrhythmicBattles.Util;

public class Binding<T> : IDisposable
{
    private readonly INotifyPropertyChanged source;
    private readonly string sourceProperty;
    private readonly object target;
    private readonly string targetProperty;
    
    private readonly Func<T> sourceGetter;
    private readonly Action<T> targetSetter;

    public Binding(INotifyPropertyChanged source, string sourceProperty, object target, string targetProperty)
    {
        this.source = source;
        this.sourceProperty = sourceProperty;
        this.target = target;
        this.targetProperty = targetProperty;
        
        // Generate getter and setter
        var sourcePropertyInfo = source.GetType().GetProperty(sourceProperty);
        var targetPropertyInfo = target.GetType().GetProperty(targetProperty);
        
        sourceGetter = (Func<T>) Delegate.CreateDelegate(typeof(Func<T>), source, sourcePropertyInfo?.GetGetMethod()!);
        targetSetter = (Action<T>) Delegate.CreateDelegate(typeof(Action<T>), target, targetPropertyInfo?.GetSetMethod()!);
        
        // Set initial value
        targetSetter(sourceGetter());
        
        // Subscribe to PropertyChanged event
        source.PropertyChanged += SourceOnPropertyChanged;
    }

    private void SourceOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == sourceProperty)
        {
            targetSetter(sourceGetter());
        }
    }

    public void Dispose()
    {
        // Unsubscribe from PropertyChanged event
        source.PropertyChanged -= SourceOnPropertyChanged;
    }
}