# NoSleep

켜 두는 동안 Windows가 유휴 절전 모드로 들어가지 않게 하는 아주 작은 트레이 앱입니다.

- 실행 파일 5KB, 설치 불필요
- 기능은 딱 하나: 실행 중에는 절전 안 함, 종료하면 원래대로
- 디스플레이는 평소 설정대로 꺼집니다 (컴퓨터만 깨어 있음)
- CPU 사용 거의 0 (타이머·폴링 없음)

## 사용법

1. [Releases](../../releases)에서 `NoSleep.exe`를 받아 실행합니다.
2. 트레이에 초록색 동그라미 아이콘이 생기면 절전 방지 중입니다.
3. 끄려면 아이콘 우클릭 → **종료**.

부팅 시 자동 실행: `Win+R` → `shell:startup` → 열린 폴더에 `NoSleep.exe` 바로가기를 넣으세요.

> 서명되지 않은 실행 파일이라 SmartScreen 경고가 뜰 수 있습니다. 걱정되면 아래처럼 소스를 직접 빌드하세요.

## 직접 빌드

Windows에 기본 포함된 .NET Framework 컴파일러를 사용합니다.

```bat
build.bat
```

## 작동 원리

시작할 때 Windows API를 한 번 호출합니다.

```csharp
SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED);
```

Windows는 입력이 없는 시간이 절전 설정값에 닿아도, "시스템 필요" 요청을 낸 프로그램이 있으면 절전하지 않습니다. 앱이 종료되면(강제 종료 포함) 요청은 자동으로 해제됩니다.

수동 절전, 전원 버튼, 노트북 덮개 닫기는 막지 않습니다.

## 라이선스

[MIT](LICENSE)
