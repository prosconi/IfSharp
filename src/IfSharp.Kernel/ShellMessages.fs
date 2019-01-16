﻿namespace IfSharp.Kernel

open System
open System.Collections.Generic
open Newtonsoft.Json

type ExecuteRequest =
    {
        // # Source code to be executed by the kernel, one or more lines.
        code: string;

        // # A boolean flag which, if True, signals the kernel to execute
        // # this code as quietly as possible.  This means that the kernel
        // # will compile the code with 'exec' instead of 'single' (so
        // # sys.displayhook will not fire), forces store_history to be False,
        // # and will *not*:
        // #   - broadcast exceptions on the PUB socket
        // #   - do any logging
        // #
        // # The default is False.
        silent: bool;

        // # A boolean flag which, if True, signals the kernel to populate history
        // # The default is True if silent is False.  If silent is True, store_history
        // # is forced to be False.
        store_history: bool;

        // # Similarly, a dict mapping names to expressions to be evaluated in the
        // # user's dict.
        user_expressions: Dictionary<string,obj>;

        // # Some frontends (e.g. the Notebook) do not support stdin requests. If
        // # raw_input is called from code executed from such a frontend, a
        // # StdinNotImplementedError will be raised.
        allow_stdin: bool;
    }

type Payload = 
    {
        html: string;
        source: string;
        start_line_number: int;
        text: string;
    }

type ExecuteReplyOk =
    {
        status: string;
        execution_count: int;

        // # 'payload' will be a list of payload dicts.
        // # Each execution payload is a dict with string keys that may have been
        // # produced by the code being executed.  It is retrieved by the kernel at
        // # the end of the execution and sent back to the front end, which can take
        // # action on it as needed.  See main text for further details.
        payload: list<Payload>;

        // # Results for the user_variables and user_expressions.
        user_variables: Dictionary<string,obj>;
        user_expressions: Dictionary<string,obj>;
    }

type ExecuteReplyError =
    {
        status: string;
        execution_count: int;

        ename: string;  // # Exception name, as a string
        evalue: string; // # Exception value, as a string

        // # The traceback will contain a list of frames, represented each as a
        // # string.  For now we'll stick to the existing design of ultraTB, which
        // # controls exception level of detail statefully.  But eventually we'll
        // # want to grow into a model where more information is collected and
        // # packed into the traceback object, with clients deciding how little or
        // # how much of it to unpack.  But for now, let's start with a simple list
        // # of strings, since that requires only minimal changes to ultratb as
        // # written.
        traceback: array<string>;
    }

type ObjectInfoRequest =
    {
        // # The (possibly dotted) name of the object to be searched in all
        // # relevant namespaces
        oname: string;

        // # The level of detail desired.  The default (0) is equivalent to typing
        // # 'x?' at the prompt, 1 is equivalent to 'x??'.
        detail_level: int;
    }

type InspectRequest =
    {
        // # The code context in which introspection is requested
        // # this may be up to an entire multiline cell.
        code: string;

        // # The cursor position within 'code' (in unicode characters) where inspection is requested
        cursor_pos: int;
        
        // # The level of detail desired.  In IPython, the default (0) is equivalent to typing
        // # 'x?' at the prompt, 1 is equivalent to 'x??'.
        // # The difference is up to kernels, but in IPython level 1 includes the source code
        // # if available.
        detail_level: int;
    }

type InspectReply =
    {
        // # 'ok' if the request succeeded or 'error', with error information as in all other replies.
        status: string;

        // # found should be true if an object was found, false otherwise
        found: bool;

        // # data can be empty if nothing is found
        data: Dictionary<string,obj>;
        metadata: Dictionary<string,obj>;
    }

type ArgsSpec =
    {
        // # The names of all the arguments
        args: array<string>;
        
        // # The name of the varargs (*args), if any
        varargs: string;
            
        // # The name of the varkw (**kw), if any
        varkw : string;
            
        // # The values (as strings) of all default arguments.  Note
        // # that these must be matched *in reverse* with the 'args'
        // # list above, since the first positional args have no default
        // # value at all.
        defaults : array<string>;
    }

type ObjectInfoReply =
    {
        // # The name the object was requested under
        name: string;

        // # Boolean flag indicating whether the named object was found or not.  If
        // # it's false, all other fields will be empty.
        found: bool;

        // # Flags for magics and system aliases
        ismagic: bool;
        isalias: bool;

        // # The name of the namespace where the object was found ('builtin',
        // # 'magics', 'alias', 'interactive', etc.)
        ``namespace``: string;

        // # The type name will be type.__name__ for normal Python objects, but it
        // # can also be a string like 'Magic function' or 'System alias'
        type_name: string;

        // # The string form of the object, possibly truncated for length if
        // # detail_level is 0
        string_form: string;

        // # For objects with a __class__ attribute this will be set
        base_class: string;

        // # For objects with a __len__ attribute this will be set
        length: int;

        // # If the object is a function, class or method whose file we can find,
        // # we give its full path
        file: string;

        // # For pure Python callable objects, we can reconstruct the object
        // # definition line which provides its call signature.  For convenience this
        // # is returned as a single 'definition' field, but below the raw parts that
        // # compose it are also returned as the argspec field.
        definition: string;

        // # The individual parts that together form the definition string.  Clients
        // # with rich display capabilities may use this to provide a richer and more
        // # precise representation of the definition line (e.g. by highlighting
        // # arguments based on the user's cursor position).  For non-callable
        // # objects, this field is empty.
        argspec: ArgsSpec;

        // # For instances, provide the constructor signature (the definition of
        // # the __init__ method):
        init_definition: string;

        // # Docstrings: for any object (function, method, module, package) with a
        // # docstring, we show it.  But in addition, we may provide additional
        // # docstrings.  For example, for instances we will show the constructor
        // # and class docstrings as well, if available.
        docstring: string;

        // # For instances, provide the constructor and class docstrings
        init_docstring: string;
        class_docstring : string;

        // # If it's a callable object whose call method has a separate docstring and
        // # definition line:
        call_def: string;
        call_docstring: string;

        // # If detail_level was 1, we also try to find the source code that
        // # defines the object, if possible.  The string 'None' will indicate
        // # that no source was found.
        source: string;
    }

type CompleteRequest = 
    {
        // # The text to be completed, such as 'a.is'
        // # this may be an empty string if the frontend does not do any lexing,
        // # in which case the kernel must figure out the completion
        // # based on 'line' and 'cursor_pos'.
        text: string;

        // # The full line, such as 'print a.is'.  This allows completers to
        // # make decisions that may require information about more than just the
        // # current word.
        line: string;

        // # The entire block of text where the line is.  This may be useful in the
        // # case of multiline completions where more context may be needed.  Note: if
        // # in practice this field proves unnecessary, remove it to lighten the
        // # messages.
        block: string;

        // # The position of the cursor where the user hit 'TAB' on the line.
        cursor_pos: int;
    }

/// Custom message used only by the web front end.
type IntellisenseRequest = {
    text: string
    line: string
    block: string
    cursor_pos: int
}

type BlockType =
    {
        selectedIndex: int;
        ch: int;
        line: int;
    }

type CompleteReplyStatus = 
    | Ok
    | Error

type CompleteReply =
    {
        // # The list of all matches to the completion request, such as
        // # ['a.isalnum', 'a.isalpha'] for the above example.
//        matches: array<string>;
        matches: obj // changed for custom UI

        // # the substring of the matched text
        // # this is typically the common prefix of the matches,
        // # and the text that is already in the block that would be replaced by the full completion.
        // # This would be 'a.is' in the above example.
        matched_text: string

        // # status should be 'ok' unless an exception was raised during the request,
        // # in which case it should be 'error', along with the usual error message content
        // # in other messages.
        status: string

        filter_start_index: int
    }

type HistoryRequest =
    {
        // # If True, also return output history in the resulting dict.
        output: bool;

        // # If True, return the raw input history, else the transformed input.
        raw: bool;

        // # So far, this can be 'range', 'tail' or 'search'.
        hist_access_type: string;

        // # If hist_access_type is 'range', get a range of input cells. session can
        // # be a positive session number, or a negative number to count back from
        // # the current session.
        session: int;
        
        // # start and stop are line numbers within that session.
        start: int;
        stop: int;

        // # If hist_access_type is 'tail' or 'search', get the last n cells.
        n: int;

        // # If hist_access_type is 'search', get cells matching the specified glob
        // # pattern (with * and ? as wildcards).
        pattern: string;

        // # If hist_access_type is 'search' and unique is true, do not
        // # include duplicated history.  Default is false.
        unique: bool;
    }

// TODO: fix this
type HistoryReply =
    {
        // # A list of 3 tuples, either:
        // # (session, line_number, input) or
        // # (session, line_number, (input, output)),
        // # depending on whether output was False or True, respectively.
        history: list<string>;
    }

type ConnectRequest = obj

type ConnectReply = 
    {
        shell_port: int;   // # The port the shell ROUTER socket is listening on.
        iopub_port: int;   // # The port the PUB socket is listening on.
        stdin_port: int;   // # The port the stdin ROUTER socket is listening on.
        hb_port: int;      // # The port the heartbeat socket is listening on.
    }

type CommOpen =
    {
        comm_id: string;        // # unique Com instance ID that has been just created by frontend
        target_name: string;    // # Which type of comm instance to construct at kernel side
        data: Dictionary<string,obj>;              // # initialization specific data
    }

type CommMessage =
    {
        comm_id: string;    // # unique Com instance ID
        data: Dictionary<string,obj>;          // # payload
    }

type CommTearDown = 
    {
        comm_id: string;    // # unique Com instance ID
        data: Dictionary<string,obj>;          // # payload
    }

type CommInfoRequest =
    {
        target_name: string; // Optional, the target name
    }

type CommInfoReply =
    {
        // A dictionary of the comms, indexed by uuids.
        comms: Dictionary<string,Dictionary<string,string>>
    }

type KernelRequest = obj

type KernelReply_LanguageInfo =
    {
        // # Name of the programming language that the kernel implements.
        // # Kernel included in IPython returns 'python'.
        name: string;

        // # Language version number.
        // # It is Python version number (e.g., '2.7.3') for the kernel
        // # included in IPython.
        version: string;

        // # mimetype for script files in this language
        mimetype: string;

        // # Extension including the dot, e.g. '.py'
        file_extension: string;

        // # Pygments lexer, for highlighting
        // # Only needed if it differs from the 'name' field.
        pygments_lexer: string;

        // # Codemirror mode, for for highlighting in the notebook.
        // # Only needed if it differs from the 'name' field.
        codemirror_mode: string;

        // # Nbconvert exporter, if notebooks written with this kernel should
        // # be exported with something other than the general 'script'
        // # exporter.
        nbconvert_exporter: string;
    }

type KernelReply_HelpLink = { text: string; url: string; }

type KernelReply =
    {
        // # Version of messaging protocol.
        // # The first integer indicates major version.  It is incremented when
        // # there is any backward incompatible change.
        // # The second integer indicates minor version.  It is incremented when
        // # there is any backward compatible change.
        protocol_version: string;
   
        // # The kernel implementation name
        // # (e.g. 'ipython' for the IPython kernel)
        implementation: string;
   
        // # Implementation version number.
        // # The version number of the kernel's implementation
        // # (e.g. IPython.__version__ for the IPython kernel)
        implementation_version: string;
   
        // # Information about the language of code for the kernel
        language_info: KernelReply_LanguageInfo;
        language: string;
   
        // # A banner of information about the kernel,
        // # which may be desplayed in console environments.
        banner : string;
   
        // # Optional: A list of dictionaries, each with keys 'text' and 'url'.
        // # These will be displayed in the help menu in the notebook UI.
        help_links: KernelReply_HelpLink array;
    }

type KernelStatus = 
    {
        // # When the kernel starts to execute code, it will enter the 'busy'
        // # state and when it finishes, it will enter the 'idle' state.
        // # The kernel will publish state 'starting' exactly once at process startup.
        // # ('busy', 'idle', 'starting')
        execution_state: string;
    }

type ShutdownRequest =
    {
        restart: bool; // # whether the shutdown is final, or precedes a restart
    }

type ShutdownReply = ShutdownRequest

type DisplayData = 
    {
        // # Who create the data
        // source: string; // Removed in protocol V5

        // # The data dict contains key/value pairs, where the kids are MIME
        // # types and the values are the raw data of the representation in that
        // # format.
        data: IDictionary<string,obj>;

        // # Any metadata that describes the data
        metadata: IDictionary<string,obj>;

        // # Optional transient data introduced in 5.1. Information not to be
        // # persisted to a notebook or other documents. Intended to live only
        // # during a live kernel session.
        transient: IDictionary<string,obj>;
    }

type Pyin = 
    {
        code: string;  // # Source code to be executed, one or more lines

        // # The counter for this execution is also provided so that clients can
        // # display it, since IPython automatically creates variables called _iN
        // # (for input prompt In[N]).
        execution_count: int;
    }

type ExecutionResult = 
    // These are identical to display_data messages, with the addition of an execution_count key.
    {
        // # The counter for this execution is also provided so that clients can
        // # display it, since IPython automatically creates variables called _N
        // # (for prompt N).
        execution_count: int;

        // # data and metadata are identical to a display_data message.
        // # the object being displayed is that passed to the display hook,
        // # i.e. the *result* of the execution.
        data: Dictionary<string,obj>;
        metadata: Dictionary<string,obj>;
        // # Optional transient data introduced in 5.1. Information not to be
        // # persisted to a notebook or other documents. Intended to live only
        // # during a live kernel session.
        transient: Dictionary<string,obj>;
    }

type Stream = 
    {
        // # The name of the stream is one of 'stdout', 'stderr'
        name: string;
        //  # The data is an arbitrary string to be written to that stream
        data: string;
    }

type ClearOutput = 
    {
        // # Wait to clear the output until new output is available.  Clears the
        // # existing output immediately before the new output is displayed.
        // # Useful for creating simple animations with minimal flickering.
        wait: bool;

        // this is undocumented!?
        // TODO: figure out if this is right
        stdout: bool;
        stderr: bool;
        other: bool;
    } 

type ShellMessage = 
    // execute
    | ExecuteRequest of ExecuteRequest
    | ExecuteReplyOk of ExecuteReplyOk
    | ExecuteReplyError of ExecuteReplyError

    // intellisense
    | InspectRequest of InspectRequest
    | InspectReply of InspectReply
    | ObjectInfoRequest of ObjectInfoRequest
    | CompleteRequest of CompleteRequest
    | IntellisenseRequest of IntellisenseRequest
    | CompleteReply of CompleteReply

    // history
    | HistoryRequest of HistoryRequest
    | HistoryReply of HistoryReply

    // connect
    | ConnectRequest of ConnectRequest
    | ConnectReply of ConnectReply

    // Comm related
    | CommOpen of CommOpen
    | CommMessage of CommMessage
    | CommTearDown of CommTearDown
    | CommInfoRequest of CommInfoRequest
    | CommInfoReply of CommInfoReply

    // kernel info
    | KernelRequest of KernelRequest
    | KernelReply of KernelReply

    // shutdown
    | ShutdownRequest of ShutdownRequest
    | ShutdownReply of ShutdownReply
    
    // input / output
    | ExecutionResult of ExecutionResult
    | DisplayData of DisplayData
    | UpdateDisplayData of DisplayData

type Header = 
    {
        msg_id: string;
        username: string;
        session: string;
        msg_type: string;
    }

type KernelMessage = 
    {
        Identifiers: list<byte[]>;
        HmacSignature: string;
        Header: Header;
        ParentHeader: Header;
        Metadata: string;
        Content: ShellMessage;
    }


module ShellMessages =

    let Deserialize (messageType:string) (messageJson:string) =
        
        match messageType with
        | "execute_request"      -> ExecuteRequest (JsonConvert.DeserializeObject<ExecuteRequest>(messageJson))
        | "execute_reply_ok"     -> ExecuteReplyOk (JsonConvert.DeserializeObject<ExecuteReplyOk>(messageJson))
        | "execute_reply_error"  -> ExecuteReplyError (JsonConvert.DeserializeObject<ExecuteReplyError>(messageJson))

        | "object_info_request"  -> ObjectInfoRequest (JsonConvert.DeserializeObject<ObjectInfoRequest>(messageJson))
        | "inspect_request"      -> InspectRequest (JsonConvert.DeserializeObject<InspectRequest>(messageJson))
        | "complete_request"     -> CompleteRequest (JsonConvert.DeserializeObject<CompleteRequest>(messageJson))
        | "complete_reply"       -> CompleteReply (JsonConvert.DeserializeObject<CompleteReply>(messageJson))

        | "intellisense_request" -> IntellisenseRequest (JsonConvert.DeserializeObject<IntellisenseRequest>(messageJson))

        | "history_request"      -> HistoryRequest (JsonConvert.DeserializeObject<HistoryRequest>(messageJson))
        | "history_reply"        -> HistoryReply (JsonConvert.DeserializeObject<HistoryReply>(messageJson))

        | "connect_request"      -> ConnectRequest (JsonConvert.DeserializeObject<ConnectRequest>(messageJson))
        | "connect_reply"        -> ConnectReply (JsonConvert.DeserializeObject<ConnectReply>(messageJson))

        | "kernel_info_request"  -> KernelRequest (JsonConvert.DeserializeObject<KernelRequest>(messageJson))
        | "kernel_info_reply"    -> KernelReply (JsonConvert.DeserializeObject<KernelReply>(messageJson))

        | "shutdown_request"     -> ShutdownRequest (JsonConvert.DeserializeObject<ShutdownRequest>(messageJson))
        | "shutdown_reply"       -> ShutdownReply (JsonConvert.DeserializeObject<ShutdownReply>(messageJson))

        | "comm_open"            -> CommOpen (JsonConvert.DeserializeObject<CommOpen>(messageJson))
        | "comm_msg"             -> CommMessage (JsonConvert.DeserializeObject<CommMessage>(messageJson))
        | "comm_close"           -> CommTearDown (JsonConvert.DeserializeObject<CommTearDown>(messageJson))

        | "comm_info_request"    -> CommInfoRequest (JsonConvert.DeserializeObject<CommInfoRequest>(messageJson))

        | _                      -> failwith ("Unsupported messageType: " + messageType)