# WMDView - 实现计划文档（分解并优先级排序的任务列表）

## [ ] Task 1: 初始化 WPF 项目结构
- **Priority**: P0
- **Depends On**: None
- **Description**:
  - 创建新的 WPF 项目
  - 配置项目基本设置（名称、命名空间等）
  - 添加 Markdig NuGet 包依赖
- **Acceptance Criteria Addressed**: [AC-1]
- **Test Requirements**:
  - `programmatic` TR-1.1: 项目能够成功编译
  - `human-judgement` TR-1.2: Markdig 包已正确安装
- **Notes**: 选择 WPF 而非 WinUI 3，因为 WPF 有更成熟的 FlowDocument 支持

## [ ] Task 2: 实现主窗口 UI 布局
- **Priority**: P0
- **Depends On**: [Task 1]
- **Description**:
  - 创建主窗口 XAML
  - 添加 FlowDocumentScrollViewer 控件铺满窗口
  - 配置窗口基本属性（大小、标题等）
- **Acceptance Criteria Addressed**: [AC-8]
- **Test Requirements**:
  - `human-judgement` TR-2.1: 窗口显示正常，FlowDocumentScrollViewer 铺满窗口
  - `human-judgement` TR-2.2: 窗口标题可以动态设置
- **Notes**:

## [ ] Task 3: 实现 Markdown 解析和基础渲染
- **Priority**: P0
- **Depends On**: [Task 2]
- **Description**:
  - 创建 Markdown 解析服务
  - 使用 Markdig 解析 Markdown 文本为 AST
  - 实现基础节点渲染（标题、段落、粗体、斜体）
  - 将渲染结果转换为 FlowDocument
- **Acceptance Criteria Addressed**: [AC-1, AC-2]
- **Test Requirements**:
  - `programmatic` TR-3.1: 能成功解析简单 Markdown 文本
  - `human-judgement` TR-3.2: 基础语法元素渲染正确
- **Notes**:

## [ ] Task 4: 实现完整 GFM 语法渲染
- **Priority**: P0
- **Depends On**: [Task 3]
- **Description**:
  - 实现列表渲染（有序、无序、任务列表）
  - 实现引用块渲染
  - 实现代码块和行内代码渲染
  - 实现表格渲染
  - 实现图片和链接渲染
  - 实现分割线渲染
- **Acceptance Criteria Addressed**: [AC-2]
- **Test Requirements**:
  - `human-judgement` TR-4.1: 所有 GFM 语法元素都能正确渲染
- **Notes**: 使用等宽字体显示代码块

## [ ] Task 5: 实现文件打开功能
- **Priority**: P0
- **Depends On**: [Task 4]
- **Description**:
  - 添加 OpenFileDialog 支持
  - 实现从文件路径读取 Markdown 内容
  - 更新窗口标题显示文件路径
  - 触发重新解析和渲染
- **Acceptance Criteria Addressed**: [AC-1, AC-8]
- **Test Requirements**:
  - `human-judgement` TR-5.1: 能通过文件对话框打开 .md 文件
  - `human-judgement` TR-5.2: 窗口标题正确显示文件路径
- **Notes**:

## [x] Task 6: 实现实时文件监听
- **Priority**: P0
- **Depends On**: [Task 5]
- **Description**:
  - 使用 FileSystemWatcher 监听当前打开的文件
  - 处理文件修改事件（防抖处理）
  - 自动重新读取、解析和渲染
- **Acceptance Criteria Addressed**: [AC-3]
- **Test Requirements**:
  - `programmatic` TR-6.1: 文件修改后 100ms 内自动刷新
- **Notes**: 添加防抖逻辑避免频繁刷新

## [x] Task 7: 实现系统主题跟随
- **Priority**: P1
- **Depends On**: [Task 2]
- **Description**:
  - 监听 Windows 系统主题变化
  - 实现亮/暗主题配色方案
  - 自动应用主题到 FlowDocument
- **Acceptance Criteria Addressed**: [AC-5]
- **Test Requirements**:
  - `human-judgement` TR-7.1: 切换系统主题时应用自动跟随变化
- **Notes**:

## [ ] Task 8: 性能优化和测试
- **Priority**: P1
- **Depends On**: [Task 6, Task 7]
- **Description**:
  - 优化渲染性能
  - 测试启动时间
  - 测试内存占用
  - 测试高分屏适配
  - 测试滚动体验
- **Acceptance Criteria Addressed**: [AC-4, AC-6, AC-7]
- **Test Requirements**:
  - `programmatic` TR-8.1: 启动时间 < 1 秒
  - `programmatic` TR-8.2: 内存占用 < 20MB
  - `human-judgement` TR-8.3: 高分屏显示清晰
  - `human-judgement` TR-8.4: 滚动流畅无卡顿
- **Notes**:

## [x] Task 9: 打包和发布准备
- **Priority**: P2
- **Depends On**: [Task 8]
- **Description**:
  - 配置发布设置
  - 生成单文件可执行程序
  - 验证体积在 2-5MB 范围内
- **Acceptance Criteria Addressed**: [NFR-2]
- **Test Requirements**:
  - `programmatic` TR-9.1: 打包后体积在 2-5MB 之间
- **Notes**: 考虑使用 .NET 单文件发布和裁剪
