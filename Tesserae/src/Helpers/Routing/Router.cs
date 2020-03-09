﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bridge;
using Retyped;
using Tesserae.Components;
using static Retyped.dom;

namespace Tesserae
{
    public static class Router
    {
        public delegate void NavigatedHandler(ActionContext toState, ActionContext fromState);
        public delegate bool CanNavigateHandler(ActionContext toState, ActionContext fromState);

        private static event NavigatedHandler onNavigated;
        private static event CanNavigateHandler onBeforeNavigate;

        private static string _lastURL = "";

        public static void OnNavigated(NavigatedHandler onNavigated)
        {
            Router.onNavigated += onNavigated;
        }

        public static void OnBeforeNavigate(CanNavigateHandler onBeforeNavigate)
        {
            //We only keep track of one active BeforeNavigate event
            foreach (Delegate d in Router.onBeforeNavigate.GetInvocationList())
            {
                Router.onBeforeNavigate -= (CanNavigateHandler)d;
            }

            if(onBeforeNavigate is null) onBeforeNavigate = (a, b) => true;

            Router.onBeforeNavigate += onBeforeNavigate;
        }

        public static void Initialize()
        {
            _initialized = true;           
        }

        private static External.Router5.Router _router;
        private static bool _initialized = false;
        private static Dictionary<string, Action<Parameters>> _routes = new Dictionary<string, Action<Parameters>>();
        private static Dictionary<string, string> _paths = new Dictionary<string, string>();

        public static void Push(string path)
        {
            if (path == window.location.href) return; //Don't double add it
            window.history.pushState(null, "", path);
            Script.Write("{0}.setState({0}.makeState('pushedState', { }, path, { }))", _router);
        }

        public static void Replace(string path)
        {
            window.history.replaceState(null, "", path);
            if (_router is object)
            {
                Script.Write("{0}.setState({0}.makeState('pushedState', { }, path, { }))", _router);
            }
        }

        public static void Navigate(string path, bool reload = false)
        {
            if (reload)
            {
                window.location.href = "./#/donothing";
            }

            window.location.href = path;
        }

        private static string LowerCasePath(string path)
        {
            var sb = new StringBuilder();
            bool inParameter = false;
            foreach(var c in path)
            {
                if (c == ':')
                {
                    inParameter = true; sb.Append(':');
                }
                else if (c == '/')
                {
                    inParameter = false;
                    sb.Append('/');
                }
                else
                {
                    if (inParameter) sb.Append(c);
                    else sb.Append(char.ToLower(c));
                }
            }
            return sb.ToString();
        }

        public static void Register(string uniqueIdentifier, string path, Func<Parameters, Task> actionTask)
        {
            Register(uniqueIdentifier, path, (p) => actionTask(p).FireAndForget());
        }

        public static void Register(string uniqueIdentifier, string path, Action<Parameters> action, bool replace = false)
        {
            // 2020-03-06 DWR: We historically lower-cased the uniqueIdentifier value here but that means that when a route-matched callback is made, the "name" value may not match the "uniqueIdentifier" that the route was
            // originally registered for. I think that the only place where casing is important is in the lowerCaseID and upperCaseID values and so I've ensured that the lowerCaseID vaue is set using ToLower() now while
            // leaving the uniqueIdentifier value untouched (as an example, if the uniqueIdentifier value may be recorded here differently to the format in which it is passed in, it would not be possible to reliably
            // implement the RouterObserver.ForRouteByName method - any name that contained upper case letters, as happens in the Tesserae Samples application side bar, would not match correctly).
            if (_routes.ContainsKey(uniqueIdentifier) && !replace)
            {
                // 2020-02-12 DWR: The last thing that the Mosaik App class does is register default routes - this means that the default routes are declared after any routes custom to the current app and this means that it
                // wouldn't be possible to have custom home pages (for example).. unless we ignore any repeat calls that specify the same uniqueIdentifier. Ignoring them allows the current app to specify a "home" route and
                // for the "home" route in the DefaultRouting.Initialize to then be ignored.
                return;
            }

            var lowerCaseID = $"path-{uniqueIdentifier.ToLower()}";
            var upperCaseID = $"PATH-{uniqueIdentifier.ToUpper()}";

            _paths.Remove(uniqueIdentifier);
            _paths.Remove(lowerCaseID);
            _paths.Remove(upperCaseID);
            _routes.Remove(uniqueIdentifier);
            _routes.Remove(lowerCaseID);
            _routes.Remove(upperCaseID);

            var lowerCasePath = LowerCasePath(path);

            if (path != lowerCasePath)
            {
                _routes[lowerCaseID] = ActionWithoutDuplicates;
                _paths[lowerCaseID] = lowerCasePath;
            }

            _routes[uniqueIdentifier] = ActionWithoutDuplicates;
            _paths[uniqueIdentifier] = path;

            Refresh();

            // This is to avoid the case of a Router Stop() & Start() on registering a new node view, that would retrigger navigation
            void ActionWithoutDuplicates(Parameters p)
            {
                if (_lastURL == window.location.href)
                {
                    return;
                }
                action(p);
                _lastURL = window.location.href;
            }
        }

        public static void Refresh(Action<dynamic, dynamic> onDone = null)
        {
            if (!_initialized) { return; }

            if (_router is object)
            {
                _router.Stop();
                _router = null;
            }

            var routes = new List<External.Router5.Route>();
            foreach (var kv in _paths)
            {
                routes.Add(new External.Router5.Route() { name = kv.Key, path = kv.Value });
            }

            var options = new External.Router5.RouteOptions() { CaseSensitive = true, QueryParamsMode = "loose" };

            _router = External.Router5.Router.New(routes.ToArray(), options);

            _router.UsePlugin(new External.Router5.BrowserPlugin(new External.Router5.BrowserPluginOptions() { UseHash = true }))
                   .UsePlugin(new External.Router5.ListenersPlugin());

            foreach (var kv in _routes)
            {
                _router.AddRouteListener(kv.Key, (state, old) => { kv.Value(state.Parameters); });
            }

            _router.Start((err, state) =>
            {
                if (err is object)
                {
                    console.log(err);
                }
                onDone?.Invoke(err, state);
            });

            _router.AddListener((toState, fromState) => onNavigated?.Invoke(toState, fromState));
            _router.CanDeactivate((to, from) => onBeforeNavigate is null ? true : onBeforeNavigate(to, from));
        }
    }


    namespace External
    {
        [GlobalMethods]
        [Scope]
        internal static class Router5
        {
            [External]
            [Name("router5")]
            public class Router
            {
                [Name("createRouter")]
                public static extern Router New(Route[] routes, RouteOptions options);

                [Name("createRouter")]
                public static extern Router New(Route[] routes);

                [Name("usePlugin")]
                public extern Router UsePlugin(object plugin);

                [Name("addListener")]
                public extern Router AddListener(Action<ActionContext, ActionContext> action);

                [Name("addNodeListener")]
                public extern Router AddNodeListener(string path, Action<ActionContext, ActionContext> action);

                [Name("addRouteListener")]
                public extern Router AddRouteListener(string path, Action<ActionContext, ActionContext> action);

                [Name("canDeactivate")]
                public extern Router CanDeactivate(Func<ActionContext, ActionContext, bool> action);


                [Name("start")]
                public extern void Start(Action<dynamic, dynamic> onDone);

                [Name("stop")]
                public extern void Stop();

                [Name("navigate")]
                public extern void Navigate(string path);

                [Name("navigate")]
                public extern void Navigate(string path, Parameters parameters);
            }

            [External]
            [Name("router5BrowserPlugin")]
            public class BrowserPlugin
            {
                public BrowserPlugin() { }
                [Template("router5BrowserPlugin({0})")]
                public BrowserPlugin(BrowserPluginOptions options) { }
            }

            [FormerInterface]
            [IgnoreCast]
            [ObjectLiteral]
            public class BrowserPluginOptions : IObject
            {
                [Name("useHash")]
                public bool UseHash;
            }

            [External]
            [Name("router5ListenersPlugin")]
            public class ListenersPlugin
            {
            }

            [FormerInterface]
            [IgnoreCast]
            [IgnoreGeneric(AllowInTypeScript = true)]
            [Virtual]
            public class RouteOptions : IObject
            {
                [Name("caseSensitive")]
                public bool CaseSensitive { get; set; }

                [Name("queryParamsMode")]
                public string QueryParamsMode { get; set; }
            }

            [FormerInterface]
            [IgnoreCast]
            [IgnoreGeneric(AllowInTypeScript = true)]
            [Virtual]
            public class Route : IObject
            {
                public string name { get; set; }
                public string path { get; set; }
                public Route[] children { get; set; }
            }
        }

        internal static class PH
        {
            public static string GetValue(object source, string name)
            {
                if (!source.HasOwnProperty(name)) throw new KeyNotFoundException();

                var value = source[name];

                if (value is null) return null;
                else if (value is string s) return s;
                return value.ToString();
            }
        }
    }

    [External]
    [ObjectLiteral]
    public class Context : IObject
    {
        public string path { get; set; }
        public string name { get; set; }
    }

    [External]
    [ObjectLiteral]
    public class ActionContext : Context
    {
        [Name("params")]
        public Parameters Parameters;
    }

    [ObjectLiteral]
    public class Parameters
    {
        // Made the constructor private so that instances are always created through FromObjectLiteral, rather than having to support mutability in this class
        private Parameters() { }
        public extern ReadOnlyArray<string> Keys { [Template("Object.getOwnPropertyNames({this})")] get; }
        public new string this[string key]
        {
            [Template("Tesserae.External.PH.GetValue({this}, {key})")]
            get
            {
                // 2019-10-01 DWR: If make this getter extern then Bridge mistranslates part of the class definition into "methods: { getItem: function (key) null }" which is invalid JS, so give it a body
                // that will be ignored (due to the Template attribute) to avoid this
                return null;
            }
        }

        public static Parameters FromObjectLiteral(object source) => (source == null) ? null : Script.Write<Parameters>("{0}", source);
    }
}
