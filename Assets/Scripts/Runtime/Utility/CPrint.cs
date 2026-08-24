using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CPrint
{
    public static bool Enable = true;
    public static bool EnableRichText = true;

    private static int _indentLevel = 0;
    private const int INDENT_SPACES = 2; 

    private static readonly HashSet<string> _onceSet = new HashSet<string>();

    // 들여쓰기 문자열
    private static string Indent
    {
        get
        {
            return new string(' ', _indentLevel * INDENT_SPACES);
        }
    }

    // 단계별 출력을 줄 맞춰서 읽기 쉽게 만듦.
    public static void IndentPush()
    {
        // 단계 올림
        _indentLevel++;
    }

    public static void IndentPop()
    {
        // 단계 내리기
        _indentLevel--;

        if (_indentLevel < 0)
        {
            _indentLevel = 0;
        }
    }

    public static void IndentReset()
    {
        _indentLevel = 0;
    }

    private enum ELogKind
    {
        Log,
        Warn,
        Error,
        Success
    }

    // 출력 포맷 관리를 위해
    // 들여쓰기 / 접두사 / 리치 텍스트 → kind 분류
    private static void Emit(ELogKind kind, string msg, string tag = null, string colorHex = null)
    {
        // 지금까지 만든 문자열을 콘솔로 내보내는 출력 코어
        if (!Enable)
        {
            return;
        }

        // 접두사 만들기 → Tag가 있으면 해당되는 프리픽스를 만듦.
        // 단, Tag가 NULL / 빈 문자열이면 접두사 없이 msg만 출력
        string prefix = string.Empty;

        if (!string.IsNullOrEmpty(tag))
        {
            // t / colorHex → Tag부분만 색을 입힘. → 가독성
            if (EnableRichText && !string.IsNullOrEmpty(colorHex))
            {
                // IsNullOrEmpty(s)
                //  ㄴ 문자열이 쓸 수 있는 값인지 검사
                //  ㄴ s == null / s == "" → t
                prefix = $"<color={colorHex}> [{tag}] </color>";
            }
            else
            {
                // (리치텍스트를 사용안하거나) 색상이 없다면 기본 형태로 만듦.
                // 공백이 있어야 msg랑 안 붙게
                prefix = $"[{tag}] ";
            }
        }

        string final = $"{Indent}{prefix}{msg}";

        switch (kind)
        {
            // 로그 종류에 맞게 통일
            case ELogKind.Log:

            case ELogKind.Success:
                Debug.Log(final);
                break;

            case ELogKind.Warn:
                Debug.LogWarning(final);
                break;

            case ELogKind.Error:
                Debug.LogError(final);
                break;
        }
    }

    // Title / Section
    public static void Title(string title, char lineCh = '=')
    {
        Line(lineCh);
        Emit(ELogKind.Log, title);
        Line(lineCh);
    }

    public static void Section(string section, char lineCh = '-')
    {
        Emit(ELogKind.Log, section);
        Line(lineCh);
    }

    public static void Line(char ch = '-', int count = 10)
    {
        Emit(ELogKind.Log, new string(ch, count));
    }

    public static void Blank(int lines = 1)
    {
        // 콘솔에 빈줄만 추가 (접두사 / 인덴트 / 색 / 태그 전부 사용 안함)
        //  ㄴ Emit 붙이면 인던트가 붙기 때문에 애매해짐.

        if (!Enable)
        {
            return;
        }

        // 빈줄 여러 줄
        if (lines <= 0)
        {
            return;
        }

        Debug.Log(new string('\n', lines));
    }

    // Log / Warn / Error
    public static void Log(string msg)
    {
        Emit(ELogKind.Log, msg);
    }

    public static void Warn(string msg)
    {
        // 주황 느낌
        Emit(ELogKind.Warn, msg, "WARN", "#FF9100");
    }

    public static void Error(string msg)
    {
        // 빨간 계열
        Emit(ELogKind.Error, msg, "ERROR", "#FF1744");
    }
    // ㄴ 경고와 달리 빨간색 → 남발 금지 → 진짜 필요한 것에만 사용

    public static void Success(string msg)
    {
        // 초록 계열
        Emit(ELogKind.Success, msg, "OK", "#00C853");
    }

    // Assert
    public static void Assert(bool condition, string msg)
    {
        // if문이 많이 빠질 수 있음 → 매번 if문 하는 것보다 Assert로 체크하면 흐름이 깔끔해짐.
        if (condition)
        {
            return;
        }

        Error($"[ASSERT] {msg}");
    }

    public static void CheckNull(object obj, string msg)
    {
        if (obj != null)
        {
            return;
        }

        Warn($"[NULL] {msg}");
    }

    public static T Ref<T>(T obj, string msg) where T : class
    {
        if (obj == null)
        {
            Warn($"[NULL] {msg}");
        }

        return obj;
    }

    // Vector3
    public static void V3(string label, Vector3 v, int digits = 2)
    {
        // 숫자 자릿수를 줄여서 로그를 읽기 쉽게 만듦.
        //  ㄴ 반올림해서 보여주면 읽을 수 있는 형태가 됨.
        //   ㄴ 라운딩 현상으로 데이터 손실이 일어남.
        float x = (float)System.Math.Round(v.x, digits);
        float y = (float)System.Math.Round(v.y, digits);
        float z = (float)System.Math.Round(v.z, digits);
        // Math.Round
        // Mathf : function 계열

        Emit(ELogKind.Log, $"{label} : ({x}, {y}, {z})");
    }

    public static void KV(string key, object value)
    {
        // KV = key = value 형태로 값을 찍는 표준 포맷 헬퍼
        // 우리가 디버깅할때 → 가장 자주 찍는 형태 → 여기서 포맷 통일함.

        Log($"{key} = {value}");
    }

    // 로그 규모가 커짐. → 섹션을 만듦. (로그 덩어리)
    public static void Group(string title, Action body, char lineCh = '=', int LineCount = 20)
    {
        if (!Enable)
        {
            return;
        }

        // 타이틀 찍고 / 들여쓰기 올리고 / 그 안의 본문 실행하고 / 들여쓰기 내리고 / 구분선으로 마무리

        // 1. 그룹 제목 출력
        Title(title, lineCh);
        // 2. 그룹 내부 → 한 단계 들여쓰기
        IndentPush();
        // 실행할 코드 블록을 호출(Action)
        // ?.Invoke() : body(함수를 가르키는 변수)가 null이면 실행하지 않음. → 예외 방지
        body?.Invoke();
        // 다시 복구 (들여쓰기)
        IndentPop();
        // 구분선 → 스타일 튜닝
        Line(lineCh, LineCount);
    }

    public static void Once(string key, string msg)
    {
        if (!Enable)
        {
            return;
        }

        // 이미 키가 있는 경우 → 재출력 금지
        if (_onceSet.Contains(key))
        {
            return;
        }

        _onceSet.Add(key);

        Warn($"[ONCE] {msg}");
    }

    public static void OnceClear()
    {
        // 등록된 키 전부 비우기
        //  ㄴ 보통 씬 재시작 → 테스트 반복 환경에서 사용할 수 있음.
        _onceSet.Clear();
    }

    // 에디터 / 개발 빌드에서만 남기고 싶은 함수 모음
    //  ㄴ 규모가 적으면 속성으로 처리 → 함수가 많아지면 → 선택적 컴파일로 처리
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Ray(Vector3 origin, Vector3 direction, Color color, float duration = 0f)
    {
        if (!Enable)
        {
            return;
        }

        Debug.DrawRay(origin, direction, color, duration);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    [System.Diagnostics.Conditional("DEVELOPMENT_BUILD")]
    public static void Line3D(Vector3 a, Vector3 b, Color color, float duration = 0f)
    {
        if (!Enable)
        {
            return;
        }

        Debug.DrawLine(a, b, color, duration);
    }

    // 이후에는 필요하면 추가 예정
    // EX : 충돌체 확인용
}
